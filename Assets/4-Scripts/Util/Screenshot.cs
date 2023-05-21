using UnityEngine;

namespace Util
{
    public class Screenshot : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                CaptureScreenshot();
            }
        }

        private void CaptureScreenshot()
        {
            // Генерируем уникальное имя для скриншота на основе текущей даты и времени
            string screenshotName = string.Format("Screenshot_{0}.png", System.DateTime.Now.ToString("yyyyMMdd_HHmmss"));

            // Создаем скриншот
            ScreenCapture.CaptureScreenshot(screenshotName);

            Debug.Log("Скриншот сохранен: " + screenshotName);
        }
    }
}