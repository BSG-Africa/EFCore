namespace Bsg.EfCore.Utils
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public class GenericReflectorService : IGenericReflectorService
    {
        #region Type
        public Type GetGenericTypeFromType(Type baseType, Type[] requiredTypesForGeneric)
        {
            return baseType.MakeGenericType(requiredTypesForGeneric);
        } 
        #endregion

        #region Action 0 parameters
        public MethodInfo GetGenericMethodFromAction(Expression<Action> expression, Type[] requiredTypesForGeneric)
        {
            var method = ((MethodCallExpression)expression.Body)
                .Method
                .GetGenericMethodDefinition();

            return this.GetGenericMethodFromMethod(method, requiredTypesForGeneric);
        }

        public void InvokeGenericMethodFromAction(
            Expression<Action> expressionWithGeneric,
            Type[] requiredTypesForGeneric,
            object instanceToInvoke)
        {
            var generifiedAction = this.GetGenericMethodFromAction(expressionWithGeneric, requiredTypesForGeneric);
            generifiedAction.Invoke(instanceToInvoke, null);
        }

        #endregion

        #region Action 1 parameters
        public MethodInfo GetGenericMethodFromAction(Expression<Action<object>> expression, Type[] requiredTypesForGeneric)
        {
            var method = ((MethodCallExpression)expression.Body)
                .Method
                .GetGenericMethodDefinition();

            return this.GetGenericMethodFromMethod(method, requiredTypesForGeneric);
        }

        public void InvokeGenericMethodFromAction(
            Expression<Action<object>> expressionWithGeneric,
            Type[] requiredTypesForGeneric,
            object instanceToInvoke)
        {
            this.InvokeGenericMethodFromAction(expressionWithGeneric, requiredTypesForGeneric, instanceToInvoke, null);
        }

        public void InvokeGenericMethodFromAction(
            Expression<Action<object>> expressionWithGeneric,
            Type[] requiredTypesForGeneric,
            object instanceToInvoke,
            object[] parametersToInvoke)
        {
            var generifiedAction = this.GetGenericMethodFromAction(expressionWithGeneric, requiredTypesForGeneric);
            generifiedAction.Invoke(instanceToInvoke, parametersToInvoke);
        } 
        #endregion

        #region Func 0 parameters
        public MethodInfo GetGenericMethodFromFunc(Expression<Func<object>> expressionWithGeneric, Type[] requiredTypesForGeneric)
        {
            var method = ((MethodCallExpression)expressionWithGeneric.Body)
                .Method
                .GetGenericMethodDefinition();

            return this.GetGenericMethodFromMethod(method, requiredTypesForGeneric);
        }

        public TResult InvokeGenericMethodFromFunc<TResult>(
            Expression<Func<object>> expressionWithGeneric,
            Type[] requiredTypesForGeneric,
            object instanceToInvoke)
        {
            var generifiedFunc = this.GetGenericMethodFromFunc(expressionWithGeneric, requiredTypesForGeneric);
            return (TResult)generifiedFunc.Invoke(instanceToInvoke, null);
        }
        #endregion

        #region Func 1 parameters
        public MethodInfo GetGenericMethodFromFunc(Expression<Func<object, object>> expressionWithGeneric, Type[] requiredTypesForGeneric)
        {
            var method = ((MethodCallExpression)expressionWithGeneric.Body)
                .Method
                .GetGenericMethodDefinition();

            return this.GetGenericMethodFromMethod(method, requiredTypesForGeneric);
        }

        public TResult InvokeGenericMethodFromFunc<TResult>(
            Expression<Func<object, object>> expressionWithGeneric,
            Type[] requiredTypesForGeneric,
            object instanceToInvoke)
        {
            return this.InvokeGenericMethodFromFunc<TResult>(expressionWithGeneric, requiredTypesForGeneric, instanceToInvoke, null);
        }

        public TResult InvokeGenericMethodFromFunc<TResult>(
            Expression<Func<object, object>> expressionWithGeneric,
            Type[] requiredTypesForGeneric,
            object instanceToInvoke,
            object[] parametersToInvoke)
        {
            var generifiedFunc = this.GetGenericMethodFromFunc(expressionWithGeneric, requiredTypesForGeneric);
            return (TResult)generifiedFunc.Invoke(instanceToInvoke, parametersToInvoke);
        }

        #endregion

        #region Method Name
        public MethodInfo GetGenericMethodFromMethodName(
            Type baseType, 
            string methodName, 
            Type[] requiredTypesForGeneric)
        {
            var entitySetMethod = baseType.GetMethod(methodName);
            return this.GetGenericMethodFromMethod(entitySetMethod, requiredTypesForGeneric);
        }

        public void InvokeGenericMethodFromMethodName(
            Type baseType, 
            string methodName, 
            Type[] requiredTypesForGeneric, 
            object instanceToInvoke)
        {
            this.InvokeGenericMethodFromMethodName(baseType, methodName, requiredTypesForGeneric, instanceToInvoke, null);
        }

        public void InvokeGenericMethodFromMethodName(
            Type baseType, 
            string methodName, 
            Type[] requiredTypesForGeneric, 
            object instanceToInvoke, 
            object[] parametersToInvoke)
        {
            var generifiedMethod = this.GetGenericMethodFromMethodName(baseType, methodName, requiredTypesForGeneric);
            generifiedMethod.Invoke(instanceToInvoke, parametersToInvoke);
        }

        public TResult InvokeGenericMethodFromMethodName<TResult>(
            Type baseType, 
            string methodName, 
            Type[] requiredTypesForGeneric, 
            object instanceToInvoke)
        {
            return this.InvokeGenericMethodFromMethodName<TResult>(baseType, methodName, requiredTypesForGeneric, instanceToInvoke, null);
        }

        public TResult InvokeGenericMethodFromMethodName<TResult>(
            Type baseType, 
            string methodName, 
            Type[] requiredTypesForGeneric, 
            object instanceToInvoke, 
            object[] parametersToInvoke)
        {
            var generifiedMethod = this.GetGenericMethodFromMethodName(baseType, methodName, requiredTypesForGeneric);
            return (TResult)generifiedMethod.Invoke(instanceToInvoke, parametersToInvoke);
        }

        #endregion

        #region Method
        public MethodInfo GetGenericMethodFromMethod(
            MethodInfo methodInfo, 
            Type[] requiredTypesForGeneric)
        {
            return methodInfo.MakeGenericMethod(requiredTypesForGeneric);
        }

        public void InvokeGenericMethodFromMethod(
            MethodInfo methodInfo, 
            Type[] requiredTypesForGeneric, 
            object instanceToInvoke)
        {
            this.InvokeGenericMethodFromMethod(methodInfo, requiredTypesForGeneric, instanceToInvoke, null);
        }

        public void InvokeGenericMethodFromMethod(
            MethodInfo methodInfo, 
            Type[] requiredTypesForGeneric, 
            object instanceToInvoke, 
            object[] parametersToInvoke)
        {
            var generifiedMethod = this.GetGenericMethodFromMethod(methodInfo, requiredTypesForGeneric);
            generifiedMethod.Invoke(instanceToInvoke, parametersToInvoke);
        }

        public TResult InvokeGenericMethodFromMethod<TResult>(
            MethodInfo methodInfo, 
            Type[] requiredTypesForGeneric, 
            object instanceToInvoke)
        {
            return this.InvokeGenericMethodFromMethod<TResult>(methodInfo, requiredTypesForGeneric, instanceToInvoke, null);
        }

        public TResult InvokeGenericMethodFromMethod<TResult>(
            MethodInfo methodInfo, 
            Type[] requiredTypesForGeneric, 
            object instanceToInvoke, 
            object[] parametersToInvoke)
        {
            var generifiedMethod = this.GetGenericMethodFromMethod(methodInfo, requiredTypesForGeneric);
            return (TResult)generifiedMethod.Invoke(instanceToInvoke, parametersToInvoke);
        }
        #endregion
    }
}
