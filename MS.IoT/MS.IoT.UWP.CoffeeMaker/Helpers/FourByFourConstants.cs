using System;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace MS.IoT.UWP.CoffeeMaker.Helpers
{
    /// <summary>
    /// Enum used for UART packet processing
    /// </summary>
    public enum ProcessingSteps { TAG, HEADER, DATA }

    /// <summary>
    /// Enum source
    /// </summary>
    public enum DataSource
    {
        FourByFour = 0,
        Other = 1
    }

    /// <summary>
    /// Constants used for UART communications
    /// </summary>
    public class FourByFourConstants
    {
        //App General
        public const int PING_INTERVAL_SECONDS = 15;
        public const int PING_INTERVAL_SECONDS_IF_DISCONNECTED = 2;

        //Serial Port
        public const int SERIAL_PORT_BAUDS = 115200;

        //UART Packet
        public const int HEADER_TAG_SIZE = 0x04;
        public const int HEADER_MESSAGE_ID_SIZE = 0x02;
        public const int HEADER_DATASIZE_SIZE = 0x02;
        public const int HEADER_COMMAND_SIZE = 0x04;
        public const int HEADER_DATA_SIZE = HEADER_MESSAGE_ID_SIZE + HEADER_DATASIZE_SIZE + HEADER_COMMAND_SIZE;
        public const int HEADER_SIZE = HEADER_TAG_SIZE + HEADER_DATA_SIZE;
        public static byte[] HEADER_TAG = { 0x46, 0x42, 0x46, 0x00 };

        //UART Constants
        public const int UART_PACKET_TIMEOUT_MS = 10000;

        //UART Commands
        public const string CMD_PING = "PING";
        public const string CMD_PONG = "PONG";
        public const string CMD_GET_PROPERTY = "CMDG";
        public const string CMD_SEND_PROPERTY = "CMDS";
        public const string CMD_CONFIRM_PROPERTY = "CMDC";
        public const string CMD_LAUNCH_ACTION = "ACTS";
        public const string CMD_CONFIRM_ACTION = "ACTC";
        public static string[] HEADER_VALID_COMMANDS = { CMD_PING, CMD_PONG, CMD_GET_PROPERTY, CMD_SEND_PROPERTY, CMD_CONFIRM_PROPERTY, CMD_LAUNCH_ACTION, CMD_CONFIRM_ACTION };

        //States
        public const string FEATURE_VARNAME_BREW_STRENGTH = "FeatureBrewStrengthActivationStatus";
        public const string FEATURE_VARNAME_BREW = "FeatureBrewActivationStatus";
        public const string FEATURE_VARNAME_GRIND_BREW = "FeatureGrindBrewActivationStatus";
        public const string FEATURE_VARNAME_WIFI = "FeatureWifiActivationStatus";
        public const string FEATURE_VARNAME_DEBUG = "FeatureDebugActivationStatus";
        public const string VALUE_STATE_BREW_STRENGTH = "StateBrewStrength"; //Reset at power on
        public const string VALUE_STATE_GRIND = "StateGrind";
        public const string VALUE_STATE_BREW = "StateActionBrew"; //Reset at power on
        public const string VALUE_STATE_BREW_ETA = "StateActionBrewETA"; //Reset at power on;
        public const string VALUE_STATE_BREW_GRIND = "StateActionGrindBrew"; //Reset at power on
        public const string VALUE_STATE_BREW_GRIND_ETA = "StateActionGrindBrewETA"; //Reset at power on;
        public const string VALUE_STAT_LAST_BREWED = "StatLastBrewed";
        public const string VALUE_STAT_NBR_BREWED_TODAY = "StatNbrBrewedToday";
        public const string VALUE_STAT_NBR_BREWED_WEEKLY = "StatNbrBrewedWeekly";
        public const string VALUE_STAT_NBR_BREWED_TOTAL = "StatNbrBrewedTotal";
        public const string VALUE_UI_CONNECTION_STATE = "ConnectionState";
        public const string VALUE_UI_MESSAGE_LOG = "MessageLog";

        //Properties
        public const string PROP_FEATURE_VARNAME_BREW_STRENGTH = "IsFeatureBrewStrengthActivated";
        public const string PROP_FEATURE_VARNAME_BREW = "IsFeatureBrewActivated";
        public const string PROP_FEATURE_VARNAME_GRIND_BREW = "IsFeatureGrindAndBrewActivated";
        public const string PROP_FEATURE_VARNAME_WIFI = "IsFeatureWifiActivated";
        public const string PROP_FEATURE_VARNAME_DEBUG = "IsFeatureDebugActivated";
        public const string PROP_VALUE_STATE_BREW_STRENGTH = "ActionBrewStrengthValue";
        public const string PROP_VALUE_STATE_GRIND = "ActionGrindValue";
        public const string PROP_VALUE_STATE_BREW = "IsActionBrewLaunched";
        public const string PROP_VALUE_STATE_BREW_ETA = "DtActionBrewETA";
        public const string PROP_VALUE_STATE_BREW_GRIND = "IsActionGrindAndBrewLaunched";
        public const string PROP_VALUE_STATE_BREW_GRIND_ETA = "DtActionGrindAndBrewETA";
        public const string PROP_VALUE_STAT_LAST_BREWED = "DtLastBrewedDate";
        public const string PROP_VALUE_STAT_NBR_BREWED_TODAY = "LblPotsBrewedToday";
        public const string PROP_VALUE_STAT_NBR_BREWED_WEEKLY = "LblPotsBrewedWeek";
        public const string PROP_VALUE_STAT_NBR_BREWED_TOTAL = "LblPotsBrewedTotal";
        public const string PROP_VALUE_UI_CONNECTION_STATE = "IsWifiConnected";

        //Methods
        public const string METHOD_CHANGE_BREW_STRENGTH = "changeBrewStrength";
        public const string METHOD_ACTION_BREW = "launchBrew";
        public const string METHOD_ACTION_GRIND_AND_BREW = "launchGrindAndBrew";
        public const string METHOD_ACTION_WIFI = "setWifi";

        //Icons
        public static ImageSource ICON_WIFI_DISCONNECTED = new BitmapImage(new Uri("ms-appx:///Assets/Images/wifi-off.png"));
        public static ImageSource ICON_WIFI_CONNECTED = new BitmapImage(new Uri("ms-appx:///Assets/Images/wifi-on.png"));

        public static ImageSource ICON_BREW_STRENGTH_DISABLED = new BitmapImage(new Uri("ms-appx:///Assets/Images/02-brew-strength-disabled.png"));
        public static ImageSource ICON_BREW_STRENGTH_REGULAR = new BitmapImage(new Uri("ms-appx:///Assets/Images/02-brew-strength-regular.png"));
        public static ImageSource ICON_BREW_STRENGTH_BOLD = new BitmapImage(new Uri("ms-appx:///Assets/Images/02-brew-strength-bold.png"));
        public static ImageSource ICON_BREW_STRENGTH_STRONG = new BitmapImage(new Uri("ms-appx:///Assets/Images/02-brew-strength-strong.png"));

        public static ImageSource ICON_BREW_INACTIVE = new BitmapImage(new Uri("ms-appx:///Assets/Images/01-brew.png"));
        public static ImageSource ICON_BREW_ACTIVE = new BitmapImage(new Uri("ms-appx:///Assets/Images/01-brew-active.png"));

        public static ImageSource ICON_GRIND_BREW_INACTIVE = new BitmapImage(new Uri("ms-appx:///Assets/Images/03-grind-and-brew.png"));
        public static ImageSource ICON_GRIND_BREW_ACTIVE = new BitmapImage(new Uri("ms-appx:///Assets/Images/03-grind-and-brew-active.png"));

        //Brew Strength Values
        public const string BREW_STRENGTH_REGULAR = "Regular";
        public const string BREW_STRENGTH_BOLD = "Bold";
        public const string BREW_STRENGTH_STRONG = "Strong";
    }
}
