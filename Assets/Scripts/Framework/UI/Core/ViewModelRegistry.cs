using System;
using System.Collections.Generic;

namespace UI.Model
{
    public static class ViewModelRegistry
    {
        private static readonly Dictionary<Type, object> _viewModels = new();

        public static void Register<T>(T vm) where T : class
        {
            _viewModels[typeof(T)] = vm;
        }

        public static T Get<T>() where T : class
        {
            if (_viewModels.TryGetValue(typeof(T), out var vm))
                return vm as T;

            return null;
        }

        public static void Clear()
        {
            _viewModels.Clear();
        }
    }
}