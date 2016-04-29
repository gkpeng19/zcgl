﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace NM.Util
{
    //http://stackoverflow.com/questions/45779/c-dynamic-event-subscription
    class EventRaiser
    {
        public event EventHandler SomethingHappened;
    }

    class Handler
    {
        public void HandleEvent() { /* ... */}
    }

    class EventProxy
    {
        static public Delegate Create(EventInfo evt, Action d)
        {
            var handlerType = evt.EventHandlerType;
            var eventParams = handlerType.GetMethod("Invoke").GetParameters();

            //lambda: (object x0, EventArgs x1) => d() 
            var parameters = eventParams.Select(p => Expression.Parameter(p.ParameterType, "x"));
            // - assumes void method with no arguments but can be 
            //   changed to accomodate any supplied method 
            var body = Expression.Call(Expression.Constant(d), d.GetType().GetMethod("Invoke"));
            var lambda = Expression.Lambda(body, parameters.ToArray());
            return Delegate.CreateDelegate(handlerType, lambda.Compile(), "Invoke", false);
        }
    }

    public class EventWrap
    {

        public static void attachEvent()
        {
            var raiser = new EventRaiser();
            var handler = new Handler();
            string eventName = "SomethingHappened";
            var eventinfo = raiser.GetType().GetEvent(eventName);
            eventinfo.AddEventHandler(raiser, EventProxy.Create(eventinfo, handler.HandleEvent));

            //or even just: 
            eventinfo.AddEventHandler(raiser, EventProxy.Create(eventinfo, () => Console.WriteLine("!")));
        }
    }
}
