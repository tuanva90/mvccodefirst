// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DynamicObjectBuilder.cs" company="Brightstar Corporation">
//   Copyright (c) Brightstar Corporation. All rights reserved.
// </copyright>
// <summary>
//   Defines the DynamicObjectBuilder type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace WPF.DataTable.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    public class DynamicObjectBuilder<TBaseClass>
        where TBaseClass : class
    {
        /// <summary>
        /// Initializes static members of the <see>
        ///                                     <cref>DynamicObjectBuilder</cref>
        ///                                   </see> class. 
        /// And so on...
        /// </summary>
        static DynamicObjectBuilder()
        {
            TypesCache = new Dictionary<TypeSignature, Type>();
            MicroModelAssemblyBuilder = AppDomain.CurrentDomain.DefineDynamicAssembly(new AssemblyName("DynamicObjects"), AssemblyBuilderAccess.Run);
            MicroModelModuleBuilder = MicroModelAssemblyBuilder.DefineDynamicModule("DynamicObjectsModule", true);
            GetValueMethod = typeof(TBaseClass).GetMethod("GetValue", BindingFlags.Instance | BindingFlags.NonPublic);
            SetValueMethod = typeof(TBaseClass).GetMethod("SetValue", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        /// <summary>
        /// Gets or sets the types cache.
        /// </summary>
        private static Dictionary<TypeSignature, Type> TypesCache { get; set; }

        /// <summary>
        /// Gets or sets the micro model assembly builder.
        /// </summary>
        private static AssemblyBuilder MicroModelAssemblyBuilder { get; set; }

        /// <summary>
        /// Gets or sets the micro model module builder.
        /// </summary>
        private static ModuleBuilder MicroModelModuleBuilder { get; set; }

        /// <summary>
        /// Gets or sets the get value method.
        /// </summary>
        private static MethodInfo GetValueMethod { get; set; }

        /// <summary>
        /// Gets or sets the set value method.
        /// </summary>
        private static MethodInfo SetValueMethod { get; set; }

        /// <summary>
        /// Gets the type of the dynamic object builder.
        /// </summary>
        /// <param name="properties">The properties.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        /// <Created>19/07/2012</Created>
        public static Type GetDynamicObjectBuilderType(IEnumerable<DataColumn> properties, string typeName = "")
        {
            if (string.IsNullOrWhiteSpace(typeName))
            {
                typeName = "DynamicObjectBuilder_" + Guid.NewGuid();
            }

            if (properties == null)
            {
                return null;
            }

            var propList = properties.ToList();

            var signature = new TypeSignature(propList);

            Type type;
            if (!TypesCache.TryGetValue(signature, out type))
            {
                type = CreateDynamicObjectBuilderType(propList, typeName);
                TypesCache.Add(signature, type);
            }

            return type;
        }

        /// <summary>
        /// Creates the type of the dynamic object builder.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="typeName">Name of the type.</param>
        /// <returns></returns>
        /// <Created>19/07/2012</Created>
        private static Type CreateDynamicObjectBuilderType(IEnumerable<DataColumn> columns, string typeName)
        {
            var typeBuilder = MicroModelModuleBuilder.DefineType(typeName, TypeAttributes.Public, typeof(TBaseClass));

            foreach (var property in columns)
            {
                var propertyBuilder = typeBuilder.DefineProperty(property.ColumnName, PropertyAttributes.None, property.DataType, null);

                CreateGetter(typeBuilder, propertyBuilder, property);
                CreateSetter(typeBuilder, propertyBuilder, property);
            }

            return typeBuilder.CreateType();
        }

        /// <summary>
        /// Creates the getter.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="propertyBuilder">The property builder.</param>
        /// <param name="column">The column.</param>
        /// <Created>19/07/2012</Created>
        private static void CreateGetter(TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, DataColumn column)
        {
            var getMethodBuilder = typeBuilder.DefineMethod(
                "get_" + column.ColumnName,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                CallingConventions.HasThis,
                column.DataType,
                Type.EmptyTypes);

            var getMethodIl = getMethodBuilder.GetILGenerator();
            getMethodIl.Emit(OpCodes.Ldarg_0);
            getMethodIl.Emit(OpCodes.Ldstr, column.ColumnName);
            getMethodIl.Emit(OpCodes.Callvirt, GetValueMethod.MakeGenericMethod(column.DataType));
            getMethodIl.Emit(OpCodes.Ret);

            propertyBuilder.SetGetMethod(getMethodBuilder);
        }

        /// <summary>
        /// Creates the setter.
        /// </summary>
        /// <param name="typeBuilder">The type builder.</param>
        /// <param name="propertyBuilder">The property builder.</param>
        /// <param name="column">The column.</param>
        /// <Created>19/07/2012</Created>
        private static void CreateSetter(TypeBuilder typeBuilder, PropertyBuilder propertyBuilder, DataColumn column)
        {
            var setMethodBuilder = typeBuilder.DefineMethod(
                "set_" + column.ColumnName,
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.SpecialName,
                CallingConventions.HasThis,
                null,
                new[] { column.DataType });

            var setMethodIl = setMethodBuilder.GetILGenerator();
            setMethodIl.Emit(OpCodes.Ldarg_0);
            setMethodIl.Emit(OpCodes.Ldstr, column.ColumnName);
            setMethodIl.Emit(OpCodes.Ldarg_1);
            setMethodIl.Emit(OpCodes.Callvirt, SetValueMethod.MakeGenericMethod(column.DataType));
            setMethodIl.Emit(OpCodes.Ret);

            propertyBuilder.SetSetMethod(setMethodBuilder);
        }
    }
}
