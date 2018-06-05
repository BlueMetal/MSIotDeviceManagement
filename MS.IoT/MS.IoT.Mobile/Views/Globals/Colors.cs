using System;
using System.Collections.Generic;
using System.Text;

using Xamarin.Forms;

namespace MS.IoT.Mobile.GlobalResources
{
    public static class Colors
    {
        // App base colours
        public const string AppBlue1 = "#185EA2";
        public const string AppBlue2 = "#0B6C9F"; 
        public const string AppGreen1 = "#29BB9C"; 
        public const string AppGreen2 = "#417505"; 
        public const string AppBrown = "#4F0E06"; 
        public const string AppRed = "#E90A17";
        public const string AppYellow = "#F49C00"; 
        public const string AppGreyDark = "#4A4A4A";
        public const string AppGreyLight1 = "#949499";
        public const string AppGreyLight2 = "#BFC9D7";
        public const string AppGreySteel = "#2E4147";
        public const string AppOffWhite = "#F8FBFF";


        public static Color FeaturesHeaderColor { get; } = Color.FromHex(AppGreySteel);
        public static Color NavBarColor { get; } = Color.FromHex(AppBlue1);
        public static Color AppliancesColor { get; } = Color.FromHex(AppBlue2);
        public static Color CheckMarksColor { get; } = Color.FromHex(AppGreen1);
        public static Color Notification1Color { get; } = Color.FromHex(AppRed);
        public static Color TextColor { get; } = Color.FromHex(AppGreyDark);
        public static Color DisabledTextColor { get; } = Color.FromHex(AppGreyLight1);
        public static Color FeaturesBackgroundColor { get; } = Color.FromHex(AppOffWhite);
        public static Color FeaturesSeperatorColor { get; } = Color.FromHex(AppGreyLight2);
        public static Color NotificationsNewFeaturesColor { get; } = Color.FromHex(AppYellow);
        public static Color NotificationsSavingsColor { get; } = Color.FromHex(AppGreen2);
        public static Color NotificationScheduleColor { get; } = Color.FromHex(AppBlue2);
        public static Color NewNotificationIndicatorColor { get; } = Color.FromHex(AppBlue1);
        public static Color NotificationBackgroundColor { get; } = Color.FromHex(AppOffWhite);
        public static Color StartButtonColor { get; } = Color.FromHex(AppGreen1);
        public static string StartButtonColorHex { get; } = AppGreen1;
        public static Color StopButtonColor { get; } = Color.FromHex(AppRed);
        public static string StopButtonColorHex { get; } = AppRed;

        public static Color MainBackgroundColor { get; } = Color.FromHex(AppBlue1);
        /*
        #185EA2 blue top header
        #0B6C9F blue (Appliances)
        #29BB9C green (checkmarks an Log In and Start buttons)
        #4F0E06 brown 
        #E90A17 red (Notification at top with a white number)
        #4A4A4A dark gray (All text)
        #949499Light gray (Disabled) 
        #2E4147 steel gray (Features background)
        #F8FBFF off white (Rows below features)
        #BFC9D7 light gray (Borders of the rows below features)
        #F49C00 yellow (Notification new features icon)
        #417505 green (Notification savings icon)
        #0B6C9F blue (Notification schedule icon)
        */




        // IBM Swatch: ultramarine 
        public const string Ultramarine1 = "#e7e9f7";
        public const string Ultramarine10 = "#d1d7f4";
        public const string Ultramarine20 = "#b0bef3";
        public const string Ultramarine30 = "#89a2f6";
        public const string Ultramarine40 = "#648fff";
        public const string Ultramarine50 = "#3c6df0";
        public const string Ultramarine60 = "#3151b7";
        public const string Ultramarine70 = "#2e3f8f";
        public const string Ultramarine80 = "#252e6a";
        public const string Ultramarine90 = "#20214f";    

        // IBM Swatch: Blue 
        public const string Blue1 = "#e1ebf7";
        public const string Blue10 = "#c8daf4";
        public const string Blue20 = "#a8c0f3";
        public const string Blue30 = "#79a6f6";
        public const string Blue40 = "#5392ff";
        public const string Blue50 = "#2d74da";
        public const string Blue60 = "#1f57a4";
        public const string Blue70 = "#25467a";
        public const string Blue80 = "#1d3458";
        public const string Blue90 = "#19273c";    

        // IBM Swatch: Cerulean 
        public const string Cerulean1 = "#deedf7";
        public const string Cerulean10 = "#c2dbf4";
        public const string Cerulean20 = "#95c4f3";
        public const string Cerulean30 = "#56acf2";
        public const string Cerulean40 = "#009bef";
        public const string Cerulean50 = "#047cc0";
        public const string Cerulean60 = "#175d8d";
        public const string Cerulean70 = "#1c496d";
        public const string Cerulean80 = "#1d364d";
        public const string Cerulean90 = "#1b2834";    

        // IBM Swatch: aqua 
        public const string Aqua1 = "#d1f0f7";
        public const string Aqua10 = "#a0e3f0";
        public const string Aqua20 = "#71cddd";
        public const string Aqua30 = "#00b6cb";
        public const string Aqua40 = "#12a3b4";
        public const string Aqua50 = "#188291";
        public const string Aqua60 = "#17616b";
        public const string Aqua70 = "#164d56";
        public const string Aqua80 = "#13393e";
        public const string Aqua90 = "#122a2e";    

        // IBM Swatch: teal 
        public const string Teal1 = "#c0f5e8";
        public const string Tea10l = "#8ee9d4";
        public const string Teal20 = "#40d5bb";
        public const string Teal30 = "#00baa1";
        public const string Teal40 = "#00a78f";
        public const string Teal50 = "#008673";
        public const string Teal60 = "#006456";
        public const string Teal70 = "#124f44";
        public const string Teal80 = "#133a32";
        public const string Teal90 = "#122b26";    

        // IBM Swatch: green 
        public const string Green1 = "#cef3d1";
        public const string Green10 = "#89eda0";
        public const string Green20 = "#57d785";
        public const string Green30 = "#34bc6e";
        public const string Green40 = "#00aa5e";
        public const string Green50 = "#00884b";
        public const string Green60 = "#116639";
        public const string Green70 = "#12512e";
        public const string Green80 = "#123b22";
        public const string Green90 = "#112c1b";    

        // IBM Swatch: lime 
        public const string Lime1 = "#d7f4bd";
        public const string Lime10 = "#b4e876";
        public const string Lime20 = "#95d13c";
        public const string Lime30 = "#81b532";
        public const string Lime40 = "#73a22c";
        public const string Lime50 = "#5b8121";
        public const string Lime60 = "#426200";
        public const string Lime70 = "#374c1a";
        public const string Lime80 = "#283912";
        public const string Lime90 = "#1f2a10";    

        // IBM Swatch: yellow 
        public const string Yellow1 = "#fbeaae";
        public const string Yellow10 = "#fed500";
        public const string Yellow20 = "#e3bc13";
        public const string Yellow30 = "#c6a21a";
        public const string Yellow40 = "#b3901f";
        public const string Yellow50 = "#91721f";
        public const string Yellow60 = "#70541b";
        public const string Yellow70 = "#5b421a";
        public const string Yellow80 = "#452f18";
        public const string Yellow = "#372118";    

        // IBM Swatch: gold 
        public const string Gold1 = "#f5e8db";
        public const string Gold10 = "#ffd191";
        public const string Gold20 = "#ffb000";
        public const string Gold30 = "#e39d14";
        public const string Gold40 = "#c4881c";
        public const string Gold50 = "#9c6d1e";
        public const string Gold60 = "#74521b";
        public const string Gold70 = "#5b421c";
        public const string Gold80 = "#42301b";
        public const string Gold90 = "#2f261c";    

        // IBM Swatch: orange 
        public const string Orange1 = "#f5e8de";
        public const string Orange10 = "#fdcfad";
        public const string Orange20 = "#fcaf6d";
        public const string Orange30 = "#fe8500";
        public const string Orange40 = "#db7c00";
        public const string Orange50 = "#ad6418";
        public const string Orange60 = "#814b19";
        public const string Orange70 = "#653d1b";
        public const string Orange80 = "#482e1a";
        public const string Orange90 = "#33241c";    

        // IBM Swatch: peach 
        public const string Peach1 = "#f7e7e2";
        public const string Peach10 = "#f8d0c3";
        public const string Peach20 = "#faad96";
        public const string Peach30 = "#fc835c";
        public const string Peach40 = "#fe6100";
        public const string Peach50 = "#c45433";
        public const string Peach60 = "#993a1d";
        public const string Peach70 = "#782f1c";
        public const string Peach80 = "#56251a";
        public const string Peach90 = "#3a201b";    

        // IBM Swatch: red 
        public const string Red1 = "#f7e6e6";
        public const string Red10 = "#fccec7";
        public const string Red20 = "#ffaa9d";
        public const string Red30 = "#ff806c";
        public const string Red40 = "#ff5c49";
        public const string Red50 = "#e62325";
        public const string Red60 = "#aa231f";
        public const string Red70 = "#83231e";
        public const string Red80 = "#5c1f1b";
        public const string Red90 = "#3e1d1b";    

        // IBM Swatch: magenta 
        public const string Magenta1 = "#f5e7eb";
        public const string Magenta10 = "#f5cedb";
        public const string Magenta20 = "#f7aac3";
        public const string Magenta30 = "#f87eac";
        public const string Magenta40 = "#ff509e";
        public const string Magenta50 = "#dc267f";
        public const string Magenta60 = "#a91560";
        public const string Magenta70 = "#831b4c";
        public const string Magenta80 = "#5d1a38";
        public const string Magenta90 = "#401a29";    

        // IBM Swatch: purple 
        public const string Purple1 = "#f7e4fb";
        public const string Purple10 = "#efcef3";
        public const string Purple20 = "#e4adea";
        public const string Purple30 = "#d68adf";
        public const string Purple40 = "#cb71d7";
        public const string Purple50 = "#c22dd5";
        public const string Purple60 = "#9320a2";
        public const string Purple70 = "#71237c";
        public const string Purple80 = "#501e58";
        public const string Purple90 = "#3b1a40";    

        // IBM Swatch: violet 
        public const string Violet1 = "#ece8f5";
        public const string Violet10 = "#e2d2f4";
        public const string Violet20 = "#d2b5f0";
        public const string Violet30 = "#bf93eb";
        public const string Violet40 = "#b07ce8";
        public const string Violet50 = "#9753e1";
        public const string Violet60 = "#7732bb";
        public const string Violet70 = "#602797";
        public const string Violet80 = "#44216a";
        public const string Violet90 = "#321c4c";    

        // IBM Swatch: indigo 
        public const string Indigo1 = "#e9e8ff";
        public const string Indigo10 = "#dcd4f7";
        public const string Indigo20 = "#c7b6f7";
        public const string Indigo30 = "#ae97f4";
        public const string Indigo40 = "#9b82f3";
        public const string Indigo50 = "#785ef0";
        public const string Indigo60 = "#5a3ec8";
        public const string Indigo70 = "#473793";
        public const string Indigo80 = "#352969";
        public const string Indigo90 = "#272149";    

        // IBM Swatch: grey 
        public const string Grey1 = "#eaeaea";
        public const string Grey10 = "#d8d8d8";
        public const string Grey20 = "#c0bfc0";
        public const string Grey30 = "#a6a5a6";
        public const string Grey40 = "#949394";
        public const string Grey50 = "#777677";
        public const string Grey60 = "#595859";
        public const string Grey70 = "#464646";
        public const string Grey80 = "#343334";
        public const string Grey90 = "#272727";    

        // IBM Swatch: cool-grey 
        public const string CoolGrey1 = "#e3ecec";
        public const string CoolGrey10 = "#d0dada";
        public const string CoolGrey20 = "#b8c1c1";
        public const string CoolGrey30 = "#9fa7a7";
        public const string CoolGrey40 = "#8c9696";
        public const string CoolGrey50 = "#6f7878";
        public const string CoolGrey60 = "#535a5a";
        public const string CoolGrey70 = "#424747";
        public const string CoolGrey80 = "#343334";
        public const string CoolGrey90 = "#272727";    

        // IBM Swatch: warm-grey 
        public const string WarmGrey1 = "#efe9e9";
        public const string WarmGrey10 = "#e2d5d5";
        public const string WarmGrey20 = "#ccbcbc";
        public const string WarmGrey30 = "#b4a1a1";
        public const string WarmGrey40 = "#9e9191";
        public const string WarmGrey50 = "#7d7373";
        public const string WarmGrey60 = "#5f5757";
        public const string WarmGrey70 = "#4b4545";
        public const string WarmGrey80 = "#373232";
        public const string WarmGrey90 = "#2a2626";    

        // IBM Swatch: neutral-white 
        public const string NeutralWhite1 = "#fcfcfc";
        public const string NeutralWhite2 = "#f9f9f9";
        public const string NeutralWhite3 = "#f6f6f6";
        public const string NeutralWhite4 = "#f3f3f3";    

        // IBM Swatch: cool-white 
        public const string CoolWhite1 = "#fbfcfc";
        public const string CoolWhite2 = "#f8fafa";
        public const string CoolWhite3 = "#f4f7f7";
        public const string CoolWhite4 = "#f0f4f4";    

        // IBM Swatch: warm-white 
        public const string WarmWhite1 = "#fdfcfc";
        public const string WarmWhite2 = "#fbf8f8";
        public const string WarmWhite3 = "#f9f6f6";
        public const string WarmWhite4 = "#f6f3f3";    


        // IBM Swatch: black 
        public const string Black = "#000000";

        // IBM Swatch: white 
        public const string White = "#ffffff";    
  
 










    }
}
