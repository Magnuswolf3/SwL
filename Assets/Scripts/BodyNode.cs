using UnityEngine;
using UnityEngine.U2D.IK;

namespace Assets.Scripts
{
    public class BodyNode : Node
    {
        // Define Private Variables
        private float _maxDist, _minDist, _followSpeed, _rotationSpeed;
        private LineRenderer _nodeLine, _rLimbLine, _lLimbLine;
        private AudioSource _rFootAudio, _lFootAudio;
        private LimbInfo _rLimbs, _lLimbs;
        private BodyLineRendererSettings _lRSettings;

        private float _legLen;
        private float _positionVariation;


        // Define shortcut getters for better readability.
        private Vector3 CurrentPosition => transform.position;
        private Vector3 ReferencePosition => prevNode.transform.position;
        private Vector3 DeltaPosition => ReferencePosition - CurrentPosition;

        // Initialization of Node Line Renderer
        void Start()
        {
            _nodeLine = gameObject.GetComponent<LineRenderer>();
            _nodeLine.sortingOrder = _lRSettings.bodySortingOrder;
            _nodeLine.startWidth = _lRSettings.bodyStartWidth;
            _nodeLine.endWidth = _lRSettings.bodyEndWidth;
        }

        // Initialization Limb Line Renderer
        private LineRenderer InitializeLineRenderer(GameObject foot)
        {
            LineRenderer lr = foot.GetComponent<LineRenderer>();
            lr.positionCount = _lRSettings.limbPositionCount;
            lr.startWidth = _lRSettings.limbStartWidth;
            lr.endWidth = _lRSettings.limbEndWidth;
            return lr;
        }

        // Initialization of private variables; Called on Awake from NodeHandler
        public void Initialize(Node prev, Node next, float maxDist, float minDist, float followSpeed, float legLen, float positionVariation, LimbInfo rLimbs, LimbInfo lLimbs, BodyLineRendererSettings lrSettings)
        {
            prevNode = prev;
            nextNode = next;
            _maxDist = maxDist;
            _minDist = minDist;
            _followSpeed = followSpeed;
            _rotationSpeed = followSpeed * 50;
            _legLen = legLen;
            _positionVariation = positionVariation;
            _rLimbs = rLimbs;
            _lLimbs = lLimbs;
            _lRSettings = lrSettings;

            tail = (nextNode == null);
            ready = true;

            _rFootAudio = _rLimbs.foot.GetComponent<AudioSource>();
            _lFootAudio = _lLimbs.foot.GetComponent<AudioSource>();

            _rLimbLine = InitializeLineRenderer(_rLimbs.foot);
            _lLimbLine = InitializeLineRenderer(_lLimbs.foot);
        }

        // Defines how the current node moves
        public override void UpdatePosition()
        {
            // If not ready or delta position is under a certain amount, don't set update target position of current node
            if (!ready || Vector3.Magnitude(DeltaPosition) < _maxDist) { return; }

            // Move current node to its target position unless its too close
            if (Vector3.Magnitude(DeltaPosition) < (_minDist + Random.Range(-_positionVariation, _positionVariation))) { return; }
            transform.position = Vector3.Lerp(CurrentPosition, TargetPosition, _followSpeed * Time.deltaTime);

            // Set Target Position with a certain degree of randomness as to where this position will lie
            if (Vector3.Magnitude(DeltaPosition) <= _maxDist) { return; }
            TargetPosition = CurrentPosition + Vector3.Normalize(DeltaPosition) * Random.Range(_minDist, _maxDist) + new Vector3(Random.Range(-_positionVariation, _positionVariation), Random.Range(-_positionVariation, _positionVariation), 0);
        }

        // Move all the Gameobjects according to the side sent through
        private void MoveSide(LimbInfo limbs, AudioSource audio, LineRenderer line, bool rightSide)
        {
            // Find the position of the feet relative to their connectors on the body
            Vector3 relFoot = limbs.foot.transform.position - limbs.origin.transform.position;

            // Calculations necessary for the placement of the elbow
            float footDist = Mathf.Sqrt(Mathf.Pow(relFoot.x, 2) + Mathf.Pow(relFoot.y, 2));
            float angle1 = Mathf.Acos((Mathf.Pow(_legLen, 2) + Mathf.Pow(footDist, 2) - Mathf.Pow(_legLen, 2)) / (2 * _legLen * footDist));

            float angle2 = Mathf.Atan2(CurrentPosition.y, CurrentPosition.x);
            
            // Angle Calculation dependant on side
            float angle3 = rightSide? angle2 + angle1: angle2 - angle1; 

            // Further calculations to figure out coordinates of the elbow
            float elbowY = _legLen * Mathf.Sin(angle3);
            float elbowX = _legLen * Mathf.Cos(angle3);

            // If the elbow reaches a state where the calculations are impossible(i.e. too stretched out), move the feet and play a noise
            // otherwise simply calculate the position of the foot
            if (float.IsNaN(elbowX) && float.IsNaN(elbowY))
            {
                limbs.foot.transform.position = limbs.placement.transform.position + new Vector3(Random.Range(-_positionVariation, _positionVariation), Random.Range(-_positionVariation, _positionVariation), 0);
                audio.pitch = Random.Range(1f, 3f);
                audio.panStereo = Random.Range(-1f, 1f);
                audio.Play();
            }
            else
            {
                limbs.elbow.transform.position = new Vector3(elbowX, elbowY, 0) + limbs.origin.transform.position;
                // Elbow local position dependant on side
                limbs.elbow.transform.localPosition = rightSide? new Vector3(Mathf.Clamp(limbs.elbow.transform.localPosition.x, 0.55f, 3 * _legLen), limbs.elbow.transform.localPosition.y, 0)
                    : new Vector3(Mathf.Clamp(limbs.elbow.transform.localPosition.x, -3 * _legLen, -0.55f), limbs.elbow.transform.localPosition.y, 0);
                line.SetPositions(new Vector3[] { limbs.foot.transform.position, limbs.elbow.transform.position, limbs.origin.transform.position });
            }
        }

        // Function to move feet if necessary otherwise positions elbows in relation to the origin of the limb and the
        // current positon of the end effector.
        private void MoveLimb()
        {
            _nodeLine.SetPositions(new Vector3[] { CurrentPosition, ReferencePosition });

            MoveSide(_rLimbs, _rFootAudio, _rLimbLine, true);
            MoveSide(_lLimbs, _lFootAudio, _lLimbLine, false);

/*          Vector3 relLFoot = _limbs.lFoot.transform.position - _limbs.lOrigin.transform.position;

            
            float lFootDist = Mathf.Sqrt(Mathf.Pow(relLFoot.x, 2) + Mathf.Pow(relLFoot.y, 2));

            
            float lAngle1 = Mathf.Acos((Mathf.Pow(_legLen, 2) + Mathf.Pow(lFootDist, 2) - Mathf.Pow(_legLen, 2)) / (2 * _legLen * lFootDist));

            
            float lAngle2 = Mathf.Atan2(CurrentPosition.y, CurrentPosition.x);

            
            float lAngle3 = lAngle2 - lAngle1;

            
            
            float lElbowY = _legLen * Mathf.Sin(lAngle3);
            float lElbowX = _legLen * Mathf.Cos(lAngle3);

            

            if (float.IsNaN(lElbowX) && float.IsNaN(lElbowY))
            {
                _limbs.lFoot.transform.position = _limbs.lPlacement.transform.position + new Vector3(Random.Range(-_positionVariation, _positionVariation), Random.Range(-_positionVariation, _positionVariation), 0);
                _lFootAudio.pitch = Random.Range(1f, 3f);
                _lFootAudio.panStereo = Random.Range(-1f, 1f);
                _lFootAudio.Play();
            }
            else
            {
                _limbs.lElbow.transform.position = new Vector3(lElbowX, lElbowY, 0) + _limbs.lOrigin.transform.position;
                _limbs.lElbow.transform.localPosition = new Vector3(Mathf.Clamp(_limbs.lElbow.transform.localPosition.x, -3 * _legLen, -0.55f), _limbs.lElbow.transform.localPosition.y, 0);
                _lLimbLine.SetPositions(new Vector3[] { _limbs.lFoot.transform.position, _limbs.lElbow.transform.position, _limbs.lOrigin.transform.position });
            }*/
        }

        // Convert to Quaternion and add required offset
        private Quaternion ConvertToQuaternion(Vector3 direction)
        {
            float dirAngle = Mathf.Atan2(direction.y, direction.x);
            Quaternion quatAngle = Quaternion.AngleAxis(dirAngle * Mathf.Rad2Deg, Vector3.forward);
            quatAngle *= Quaternion.Euler(0, 0, 90);

            return quatAngle;
        }

        // Rotates the node to face the direction its moving in.
        private void RotateNode()
        {
            Vector3 deltaRotation = (DeltaPosition) / Vector3.Magnitude(DeltaPosition);
            Quaternion requiredAngle = ConvertToQuaternion(deltaRotation);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, requiredAngle, Time.deltaTime * _rotationSpeed);
        }

        // Calls main functions
        void Update()
        {
            // Rotate the node and move the feet
            RotateNode();
            MoveLimb();
            UpdatePosition();
        }
    }
}