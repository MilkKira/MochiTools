using EFT.Communications;

namespace MochiTools.Notify
{
    public class MochiNotify
    {
        public static void OnOff(string name, bool state)
        {
            if (state)
                NotificationManager.DisplaySingletonNotification($"{name} ENABLED");
            else
                NotificationManager.DisplaySingletonWarningNotification($"{name} DISABLED");
        }

        // 普通提示
        public static void Msg(string msg)
        {
            NotificationManager.DisplayMessageNotification(msg);
        }
    }
}