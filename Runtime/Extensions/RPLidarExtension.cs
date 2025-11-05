using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace IOToolkit.RPLidar
{
    public static class RPLidarExtension
    {
        private static Vector2 touchPoint = new Vector2(-1, -1);

        public static void EnableDebugWindow(this IODevice rplidarDevice, int debug)
        {
            rplidarDevice.SetDO(IOKeyCode.OAxis_240, debug);
        }

        /// <summary>
        /// 获取指定区域的触摸点
        /// </summary>
        /// <param name="rplidarDevice"></param>
        /// <param name="areaIndex">雷达区域索引 从0开始 </param>
        /// <param name="pointerIndex">雷达触摸点索引 从0开始 </param>
        /// <returns></returns>
        public static Vector2 GetTouchPoint(
            this IODevice rplidarDevice,
            int areaIndex,
            int pointerIndex
        )
        {
            var _keyIndex = areaIndex * 32 + pointerIndex * 2;
            var xKeyName = $"Axis_{_keyIndex.ToString().PadLeft(2, '0')}";
            var yKeyName = $"Axis_{(_keyIndex + 1).ToString().PadLeft(2, '0')}";

            var _x = rplidarDevice.GetAxisKey(xKeyName) * 1000;
            var _y = rplidarDevice.GetAxisKey(yKeyName) * 1000;

            touchPoint.x = _x;
            touchPoint.y = Screen.height - _y;
            return touchPoint;
        }

        public static void SimulateMouseMove(this IODevice rplidarDevice, int areaIndex)
        {
            List<GameObject> _hoveredObjects = new List<GameObject>();
            IOToolkit.Instance.OnUpdateEvent.AddListener(() =>
            {
                var _touchPosition = rplidarDevice.GetTouchPoint(areaIndex, 0);
                var _eventData = new PointerEventData(EventSystem.current)
                {
                    position = _touchPosition
                };
                List<RaycastResult> results = new List<RaycastResult>();
                EventSystem.current.RaycastAll(_eventData, results);

                var _newEnterObjects = results
                    .Select(result => result.gameObject)
                    .Where(_obj => !_hoveredObjects.Contains(_obj))
                    .ToList();

                _newEnterObjects.ForEach(_obj =>
                {
                    ExecuteEvents.Execute(_obj, _eventData, ExecuteEvents.pointerEnterHandler);
                });

                var _exitObjects = _hoveredObjects.Except(
                    results.Select(result => result.gameObject)
                );

                foreach (GameObject _exitObject in _exitObjects)
                {
                    ExecuteEvents.Execute(
                        _exitObject,
                        _eventData,
                        ExecuteEvents.pointerExitHandler
                    );
                }
                _hoveredObjects = results.Select(result => result.gameObject).ToList();
            });
        }
    }
}
