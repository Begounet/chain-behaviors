using AUE;
using UnityEngine;
using UnityEngine.EventSystems;

namespace ChainBehaviors.UI
{
    public class PointerInteractableArea : BaseMethod, 
        IPointerEnterHandler, IPointerDownHandler, IPointerClickHandler, 
        IPointerMoveHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField]
        private bool _normalizePosition = true;

        [SerializeField]
        private bool _clamp = true;

        [SerializeField]
        private AUEEvent<PointerEventData, Vector2>  _onPointerEnter;

        [SerializeField]
        private AUEEvent<PointerEventData, Vector2> _onPointerExit;

        [SerializeField]
        private AUEEvent<PointerEventData, Vector2> _onPointerUp;

        [SerializeField]
        private AUEEvent<PointerEventData, Vector2> _onPointerDown;

        [SerializeField]
        private AUEEvent<PointerEventData, Vector2> _onPointerMove;

        [SerializeField]
        private AUEEvent<PointerEventData, Vector2> _onPointerClick;

        public void OnPointerClick(PointerEventData eventData) => RaiseEvent("On Pointer Click", _onPointerClick, eventData);
        public void OnPointerDown(PointerEventData eventData) => RaiseEvent("On Pointer Down", _onPointerDown, eventData);
        public void OnPointerEnter(PointerEventData eventData) => RaiseEvent("On Pointer Enter", _onPointerEnter, eventData);
        public void OnPointerExit(PointerEventData eventData) => RaiseEvent("On Pointer Exit", _onPointerExit, eventData);
        public void OnPointerMove(PointerEventData eventData) => RaiseEvent("On Pointer Move", _onPointerMove, eventData);
        public void OnPointerUp(PointerEventData eventData) => RaiseEvent("On Pointer Up", _onPointerUp, eventData);

        private void RaiseEvent(string eventName, AUEEvent<PointerEventData, Vector2> evt, PointerEventData eventData)
        {
            Vector2 pos = eventData.position;
            var rectTrans = GetComponent<RectTransform>();
            Camera camera = eventData.pressEventCamera;
            if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTrans, pos, camera, out Vector2 localPosition))
            {
                return;
            }

            if (_normalizePosition)
            {
                localPosition = NormalizePosition(localPosition);
            }
            if (_clamp)
            {
                localPosition = ClampPosition(localPosition);
            }
            TraceCustomMethodName(eventName, ("eventData", eventData), ("position", localPosition));
            evt.Invoke(eventData, localPosition);
        }

        private Vector2 ClampPosition(Vector2 localPoint)
        {
            Vector2 maxXY = Vector2.zero;

            if (_normalizePosition)
            {
                maxXY = new Vector2(1, 1);
            }
            else
            {
                var rectTrans = (RectTransform)transform;
                Rect rect = rectTrans.rect;
                maxXY = new Vector2(rect.width, rect.height);
            }
            localPoint.x = Mathf.Clamp(localPoint.x, 0, maxXY.x);
            localPoint.y = Mathf.Clamp(localPoint.y, 0, maxXY.y);
            return localPoint;
        }

        private Vector2 NormalizePosition(Vector2 pos)
        {
            var rectTrans = (RectTransform)transform;
            Rect rect = rectTrans.rect;
            pos.x -= rect.x;
            pos.y -= rect.y;
            return (pos / rect.size);
        }
    }
}
