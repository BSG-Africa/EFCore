namespace Bsg.EfCore.Utils
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    public interface IGenericReflectorService
    {
        Type GetGenericTypeFromType(Type baseType, Type[] requiredTypesForGeneric);

        MethodInfo GetGenericMethodFromAction(Expression<Action<object>> expression, Type[] requiredTypesForGeneric);

        MethodInfo GetGenericMethodFromAction(Expression<Action> expression, Type[] requiredTypesForGeneric);

        MethodInfo GetGenericMethodFromFunc(Expression<Func<object, object>> expressionWithGeneric, Type[] requiredTypesForGeneric);

        MethodInfo GetGenericMethodFromFunc(Expression<Func<object>> expressionWithGeneric, Type[] requiredTypesForGeneric);

        void InvokeGenericMethodFromAction(Expression<Action<object>> expressionWithGeneric, Type[] requiredTypesForGeneric, object instanceToInvoke);

        void InvokeGenericMethodFromAction(Expression<Action<object>> expressionWithGeneric, Type[] requiredTypesForGeneric, object instanceToInvoke, object[] parametersToInvoke);

        void InvokeGenericMethodFromAction(Expression<Action> expressionWithGeneric, Type[] requiredTypesForGeneric, object instanceToInvoke);

        TResult InvokeGenericMethodFromFunc<TResult>(Expression<Func<object, object>> expressionWithGeneric, Type[] requiredTypesForGeneric, object instanceToInvoke);

        TResult InvokeGenericMethodFromFunc<TResult>(Expression<Func<object, object>> expressionWithGeneric, Type[] requiredTypesForGeneric, object instanceToInvoke, object[] parametersToInvoke);

        TResult InvokeGenericMethodFromFunc<TResult>(Expression<Func<object>> expressionWithGeneric, Type[] requiredTypesForGeneric, object instanceToInvoke);

        MethodInfo GetGenericMethodFromMethodName(Type baseType, string methodName, Type[] requiredTypesForGeneric);

        void InvokeGenericMethodFromMethodName(Type baseType, string methodName, Type[] requiredTypesForGeneric, object instanceToInvoke);

        void InvokeGenericMethodFromMethodName(Type baseType, string methodName, Type[] requiredTypesForGeneric, object instanceToInvoke, object[] parametersToInvoke);

        TResult InvokeGenericMethodFromMethodName<TResult>(Type baseType, string methodName, Type[] requiredTypesForGeneric, object instanceToInvoke);

        TResult InvokeGenericMethodFromMethodName<TResult>(Type baseType, string methodName, Type[] requiredTypesForGeneric, object instanceToInvoke, object[] parametersToInvoke);

        MethodInfo GetGenericMethodFromMethod(MethodInfo methodInfo, Type[] requiredTypesForGeneric);

        void InvokeGenericMethodFromMethod(MethodInfo methodInfo, Type[] requiredTypesForGeneric, object instanceToInvoke);

        void InvokeGenericMethodFromMethod(MethodInfo methodInfo, Type[] requiredTypesForGeneric, object instanceToInvoke, object[] parametersToInvoke);

        TResult InvokeGenericMethodFromMethod<TResult>(MethodInfo methodInfo, Type[] requiredTypesForGeneric, object instanceToInvoke);

        TResult InvokeGenericMethodFromMethod<TResult>(MethodInfo methodInfo, Type[] requiredTypesForGeneric, object instanceToInvoke, object[] parametersToInvoke);
    }
}