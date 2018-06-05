using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Reflection;
using System.Windows.Input;

using Xamarin.Forms;

namespace QXFUtilities
{

    /// <summary>
    /// Based Upon EventToCmmand in Xamarin.Forms by Anthony Simmon https://anthonysimmon.com/eventtocommand-in-xamarin-forms-apps/
    /// </summary>
    public class EventToCommandBehaviour : BindableBehaviour<VisualElement>//<View>
    {
        public static readonly BindableProperty EventNameProperty = BindableProperty.Create(nameof(EventName), typeof(string), typeof(EventToCommandBehaviour), null);
        public static readonly BindableProperty CommandProperty = BindableProperty.Create(nameof(Command), typeof(ICommand), typeof(EventToCommandBehaviour), null);
        public static readonly BindableProperty CommandParameterProperty = BindableProperty.Create(nameof(CommandParameter), typeof(object), typeof(EventToCommandBehaviour), null);
        public static readonly BindableProperty EventArgsConverterProperty = BindableProperty.Create(nameof(EventArgsConverter), typeof(IValueConverter), typeof(EventToCommandBehaviour), null);
        public static readonly BindableProperty EventArgsConverterParameterProperty = BindableProperty.Create(nameof(EventArgsConverterParameter), typeof(object), typeof(EventToCommandBehaviour), null);
        
        private Delegate _handler;
        private EventInfo _eventInfo;

        public string EventName
        {
            get { return (string)GetValue(EventNameProperty); }
            set { SetValue(EventNameProperty, value); }
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        public IValueConverter EventArgsConverter
        {
            get { return (IValueConverter)GetValue(EventArgsConverterProperty); }
            set { SetValue(EventArgsConverterProperty, value); }
        }

        public object EventArgsConverterParameter
        {
            get { return GetValue(EventArgsConverterParameterProperty); }
            set { SetValue(EventArgsConverterParameterProperty, value); }
        }

        protected override void OnAttachedTo(VisualElement visualElement)//(View visualElement)
        {
            base.OnAttachedTo(visualElement);

            var events = AssociatedObject.GetType().GetRuntimeEvents().ToArray();
            if (events.Any())
            {
                _eventInfo = events.FirstOrDefault(e => e.Name == EventName);
                if (_eventInfo == null)
                    throw new ArgumentException(String.Format("EventToCommand: Can't find any event named '{0}' on attached type", EventName));

                AddEventHandler(_eventInfo, AssociatedObject, OnFired);
            }
        }

        protected override void OnDetachingFrom(VisualElement visualElement)
        {
            if (_handler != null)
                _eventInfo.RemoveEventHandler(AssociatedObject, _handler);

            base.OnDetachingFrom(visualElement);
        }

        private void AddEventHandler(EventInfo eventInfo, object item, Action<object, EventArgs> action)
        {
            var eventParameters = eventInfo.EventHandlerType
                .GetRuntimeMethods().First(m => m.Name == "Invoke")
                .GetParameters()
                .Select(p => Expression.Parameter(p.ParameterType))
                .ToArray();

            var actionInvoke = action.GetType()
                .GetRuntimeMethods().First(m => m.Name == "Invoke");

            _handler = Expression.Lambda(
                eventInfo.EventHandlerType,
                Expression.Call(Expression.Constant(action), actionInvoke, eventParameters[0], eventParameters[1]),
                eventParameters
            )
            .Compile();

            eventInfo.AddEventHandler(item, _handler);
        }

        private void OnFired(object sender, EventArgs eventArgs)
        {
            if (Command == null)
                return;

            var parameter = CommandParameter;

            if (eventArgs != null && eventArgs != EventArgs.Empty)
            {
                parameter = eventArgs;

                if (EventArgsConverter != null)
                {
                    parameter = EventArgsConverter.Convert(eventArgs, typeof(object), EventArgsConverterParameter, CultureInfo.CurrentUICulture);
                }
            }

            if (Command.CanExecute(parameter))
            {
                Command.Execute(parameter);
            }
        }
    }
}
