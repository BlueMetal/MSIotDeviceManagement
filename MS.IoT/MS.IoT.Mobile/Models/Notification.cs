using System;
using System.Collections.Generic;
using System.Text;

namespace MS.IoT.Mobile
{
    public class Notification
    {
        public NotificationType NotificationType { get; set; }

        public string Id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; }

        public string Message { get; set; }

        public DateTime ReceivedDTS { get; set; }

        public bool Read { get; set; } = false;

        public string DeviceId  { get; set; } = null;

        public string FeatureName { get; set; } = null;

        public string FeatureId { get; set; } = null;

        public FeatureType FeatureType { get; set; } = FeatureType.Action;

        public SubType SubType { get; set; } = SubType.Undefined;

        public string MethodName { get; set; } = null;

        public string UrlLink { get; set; } = null;

        public bool Actioned { get; set; } = false;

        public string CustomIcon { get; set; } = null;



        public Notification() { }
 
        public Notification(PushNotificationDto dto, DateTime? received = null)
        {
            NotificationType ntype = NotificationType.Undefined;
            if ( Enum.TryParse<NotificationType>(dto.NotificationType, out ntype))
            {
                NotificationType = ntype;
                DeviceId = dto.DeviceId;
                FeatureId = dto.FeatureId;
                FeatureName = dto.Name;
                MethodName = dto.MethodName;
                Title = dto.Description;
                Message = dto.Message;
                UrlLink = dto.LinkUrl;
                CustomIcon = dto.CustomIcon;
                if (received == null)
                    ReceivedDTS = DateTime.Now;
                else
                    ReceivedDTS = (DateTime)received;
                // Server model currently doesn't include the Feature Type, so will need to infer
                if (string.IsNullOrWhiteSpace(dto.FeatureType))
                {
                    // get the type - this is hard coded as the server data model doesn't support his
                    if (dto.MethodName.Contains("launch"))
                        FeatureType = FeatureType.Action;
                    else if (dto.MethodName.Contains("change"))
                    {
                        FeatureType = FeatureType.Selection;
                        var index = dto.MethodName.IndexOf("change");
                        string type = dto.MethodName.Remove(index, "change".Length);
                        SubType st = SubType.BrewStrength;  //SubType.Undefined;
                        if (Enum.TryParse(type, out st))
                            SubType = st;
                        else
                            FeatureType = FeatureType.Undefined;
                    }
                }
                else
                {
                    switch (dto.FeatureType)
                    {
                        case "Selection" :
                            FeatureType = FeatureType.Selection;
                            break;

                        case "SubType" :
                            FeatureType = FeatureType.SubType;
                            break;

                        default:
                            FeatureType = FeatureType.Action;
                            break;
                    }
                }
            }
        }


    }


    public enum NotificationType
    {
        Feature,
        FeatureUpdate,
        Message,
        Marketing,
        Announcement,
        Offer,
        Maintenance,
        Schedule,
        Undefined
    }




}
