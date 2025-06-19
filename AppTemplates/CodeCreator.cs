using System;
using System.Reflection;
using System.ComponentModel.Design;
using System.Linq;
using System.Collections.Generic;

namespace NanoGo.AppTemplates
{
    public class CodeCreator
    {
        private string _serviceTemplate;
        private string _appNameSpace;
        private string _contextName;
        private string _entityName;
        private string _sEntityName;
        private string _primaryKey;

        public CodeCreator()
        {

        }
        public CodeCreator(string serviceTemplate, string appNameSpace, string contextName, string entityName, string primaryKey)
        {
            _serviceTemplate = serviceTemplate;
            _appNameSpace = appNameSpace;
            _contextName = contextName;
            _entityName = entityName;
            _primaryKey = primaryKey;
            _sEntityName = char.ToLower(entityName[0]) + entityName.Substring(1);
        }

        public CodeCreator SetServiceTemplate(string serviceTemplate)
        {
            _serviceTemplate = serviceTemplate;
            return this;
        }
        public CodeCreator SetAppNamepace(string appNameSpace)
        {
            _appNameSpace = appNameSpace;
            return this;
        }
        public CodeCreator SetContextName(string contextName)
        {
            _contextName = contextName;
            return this;
        }
        public CodeCreator SetPrimaryKey(string primaryKey)
        {
            _primaryKey = primaryKey;
            return this;
        }
        public CodeCreator SetEntityName(string entityName)
        {
            _entityName = entityName;
            _sEntityName = char.ToLower(entityName[0]) + entityName.Substring(1);
            return this;
        }

        public string CreateServiceCode()
        {
            string serviceTemplateCopy = _serviceTemplate;

            serviceTemplateCopy = serviceTemplateCopy.Replace("@AppNamespace@", _appNameSpace);
            serviceTemplateCopy = serviceTemplateCopy.Replace("@Entity@", _entityName);
            serviceTemplateCopy = serviceTemplateCopy.Replace("@SEntity@", _sEntityName);
            serviceTemplateCopy = serviceTemplateCopy.Replace("@PrimaryKey@", _primaryKey);
            serviceTemplateCopy = serviceTemplateCopy.Replace("@Context@", _contextName);
            serviceTemplateCopy = serviceTemplateCopy.Replace("@AppNamespace@", _appNameSpace);
            var test = Type.GetType(_appNameSpace + ".Models." + _entityName)
               .GetProperties();
            var filterList = Type.GetType(_appNameSpace + ".Models." + _entityName)
                .GetProperties()
                .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
                .Select(f =>
                {
                    if (f.PropertyType.ToString() == "System.String")
                        return $"if (!string.IsNullOrEmpty(filter.{f.Name})) dbQuery = dbQuery.Where(x => x.{f.Name}.Contains(filter.{f.Name}));";
                    if (f.PropertyType.ToString().Contains("DateTime"))
                        return $"if (filter.{f.Name}.HasValue) dbQuery = dbQuery.Where(x => x.{f.Name} >= filter.{f.Name}.Value);\n" +
                               $"if (filter.{f.Name}2.HasValue) dbQuery = dbQuery.Where(x => x.{f.Name} <= filter.{f.Name}2.Value);";
                    else
                        return $"if (filter.{f.Name}.HasValue) dbQuery = dbQuery.Where(x => x.{f.Name} == filter.{f.Name}.Value);";

                })
                .ToList();

            serviceTemplateCopy = serviceTemplateCopy.Replace("@Filters@", string.Join("\n", filterList));

            var selectList = Type.GetType(_appNameSpace + ".Models." + _entityName)
               .GetProperties()
                .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
               .Select(f =>
               {
                   return $"{f.Name} = x.{f.Name}";

               })
               .ToList();

            serviceTemplateCopy = serviceTemplateCopy.Replace("@SelectFields@", string.Join(",\n", selectList));

            selectList = Type.GetType(_appNameSpace + ".Models." + _entityName)
              .GetProperties()
                 .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
              .Select(f =>
              {
                  if (!(new List<string> { _primaryKey, "UpdatedDate", "UpdatedUser", "CreatedDate", "CreatedUser" }).Any(x => x == f.Name))
                  {
                      return $"{_sEntityName}.{f.Name} = {_sEntityName}DTO.{f.Name} ;";
                  }
                  return "";

              })
              .ToList();

            serviceTemplateCopy = serviceTemplateCopy.Replace("@SaveSetFields@", string.Join("\n", selectList));



            selectList = Type.GetType(_appNameSpace + ".Models." + _entityName)
            .GetProperties()
              .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
            .Select(f =>
            {
                if (f.PropertyType.ToString().Contains("Int32"))
                    return "public Int32" + (f.PropertyType.ToString().Contains("Nullable") ? "?" : "") + " " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("Int64"))
                    return "public Int64" + (f.PropertyType.ToString().Contains("Nullable") ? "?" : "") + " " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("Boolean"))
                    return "public Boolean" + (f.PropertyType.ToString().Contains("Nullable") ? "?" : "") + " " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("DateTime"))
                    return "public DateTime" + (f.PropertyType.ToString().Contains("Nullable") ? "?" : "") + " " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("Decimal"))
                    return "public Decimal" + (f.PropertyType.ToString().Contains("Nullable") ? "?" : "") + " " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("Single"))
                    return "public float" + (f.PropertyType.ToString().Contains("Nullable") ? "?" : "") + " " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("Double"))
                    return "public Double" + (f.PropertyType.ToString().Contains("Nullable") ? "?" : "") + " " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("String"))
                    return "public String " + f.Name.ToString() + " { get; set; }";
                else
                    return "public " + f.Name.ToString() + " { get; set; }";

            })
            .ToList();

            serviceTemplateCopy = serviceTemplateCopy.Replace("@EntityDTOFields@", string.Join("\n", selectList));

            selectList = Type.GetType(_appNameSpace + ".Models." + _entityName)
            .GetProperties()
              .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
            .Select(f =>
            {
                if (f.PropertyType.ToString().Contains("Int32"))
                    return "public Int32? " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("Int64"))
                    return "public Int64? " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("Boolean"))
                    return "public Boolean? " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("DateTime"))
                    return "public DateTime? " + f.Name.ToString() + " { get; set; }\n" +
                           "public DateTime? " + f.Name.ToString() + "2 { get; set; }";
                if (f.PropertyType.ToString().Contains("Decimal"))
                    return "public Decimal? " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("Single"))
                    return "public float? " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("Double"))
                    return "public Double? " + f.Name.ToString() + " { get; set; }";
                if (f.PropertyType.ToString().Contains("String"))
                    return "public String " + f.Name.ToString() + " { get; set; }";
                else
                    return "public " + f.Name.ToString() + " { get; set; }";

            })
            .ToList();

            serviceTemplateCopy = serviceTemplateCopy.Replace("@EntityFilterFields@", string.Join("\n", selectList));

            return serviceTemplateCopy;
        }

        public string CreateContollerCode()
        {
            string serviceTemplateCopy = _serviceTemplate;

            serviceTemplateCopy = serviceTemplateCopy.Replace("@AppNamespace@", _appNameSpace);
            serviceTemplateCopy = serviceTemplateCopy.Replace("@Entity@", _entityName);
            serviceTemplateCopy = serviceTemplateCopy.Replace("@SEntity@", _sEntityName);
            serviceTemplateCopy = serviceTemplateCopy.Replace("@PrimaryKey@", _primaryKey);
            serviceTemplateCopy = serviceTemplateCopy.Replace("@Context@", _contextName);

            return serviceTemplateCopy;
        }


       

    }
}

