namespace Meyer.Socrates.Data
{
    using System;

    [Flags]
    public enum SocPropertyFlags
    {
        KeepCache = 1,
        SilenceAllNotifications = SilenceChangingNotifications | SilenceChangedNotifications,
        SilenceChangingNotifications = 2,
        SilenceChangedNotifications = 4,
    }
}
