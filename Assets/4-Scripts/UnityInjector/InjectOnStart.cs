namespace JohanPolosn.UnityInjector
{
    using System;
    using JohanPolosn.UnityInjector.Internals;
    using UnityEngine;

    [AddComponentMenu("Unity Injector/Inject On Start")]
    public class InjectOnStart : OnEventInjector
    {
        private void Start()
        {
            this.InjectComponents();
        }

        protected override Exception GetException(string message)
        {
            return new InjectOnStartException(message);
        }
    }

}