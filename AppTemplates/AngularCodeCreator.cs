using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.AppTemplates.AngularTemplates
{
    public class AngularCodeCreator
    {
        private string _serviceTSTemplate;
        private string _componentHTMLTemplate;
        private string _componentTSTemplate;
        private string _dialogComponentHTMLTemplate;
        private string _dialogComponentTSTemplate;
        private string _appNameSpace;
        private string _contextName;
        private string _entityName;
        private string _sEntityName;
        private string _primaryKey;

        public AngularCodeCreator()
        {

        }
        public AngularCodeCreator(string componentHTMLTemplate, string appNameSpace, string contextName, string entityName, string primaryKey)
        {
            _componentHTMLTemplate = componentHTMLTemplate;
            _appNameSpace = appNameSpace;
            _contextName = contextName;
            _entityName = entityName;
            _primaryKey = primaryKey;
            _sEntityName = char.ToLower(entityName[0]) + entityName.Substring(1);
        }
        public AngularCodeCreator SetAppNamepace(string appNameSpace)
        {
            _appNameSpace = appNameSpace;
            return this;
        }
        public AngularCodeCreator SetContextName(string contextName)
        {
            _contextName = contextName;
            return this;
        }
        public AngularCodeCreator SetPrimaryKey(string primaryKey)
        {
            _primaryKey = primaryKey;
            return this;
        }

        public AngularCodeCreator SetEntityName(string entityName)
        {
            _entityName = entityName;
            _sEntityName = char.ToLower(entityName[0]) + entityName.Substring(1);
            return this;
        }
        public AngularCodeCreator SetServiceTSTemplate(string serviceTSTemplate)
        {
            _serviceTSTemplate = serviceTSTemplate;
            return this;
        }
        public AngularCodeCreator SetComponentHTMLTemplate(string componentHTMLTemplate)
        {
            _componentHTMLTemplate = componentHTMLTemplate;
            return this;

        }
        public AngularCodeCreator SetComponentTSTemplate(string componentTSTemplate)
        {
            _componentTSTemplate = componentTSTemplate;
            return this;
        }
        public AngularCodeCreator SetDialogComponentHTMLTemplate(string dialogComponentHTMLTemplate)
        {
            _dialogComponentHTMLTemplate = dialogComponentHTMLTemplate;
            return this;
        }
        public AngularCodeCreator SetDialogComponentTSTemplate(string dialogComponentTSTemplate)
        {
            _dialogComponentTSTemplate = dialogComponentTSTemplate;
            return this;
        }


        public string CreateComponentHTMLClientCode()
        {
            string componentHTMLTemplateCopy = _componentHTMLTemplate;
            var entityTranslateString = _entityName.ToUpperInvariant();

            componentHTMLTemplateCopy = componentHTMLTemplateCopy.Replace("@ENTITYUPPER@", entityTranslateString);

            var columnList = Type.GetType(_appNameSpace + ".Models." + _entityName)
                .GetProperties()
                .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
                .Where(f => f.Name != "CreatedDate" && f.Name != "CreatedUser")
                .Select(f =>
                {
                    string translateString = f.Name.ToUpperInvariant();
                    string clientName = f.Name.Substring(0, 1).ToLowerInvariant() + f.Name.Substring(1, f.Name.Length - 1);

                    if (f.PropertyType.ToString().Contains( "Boolean"))
                        return $"<ng-container matColumnDef=\"{clientName}\">\n" +
                                $"   <mat-header-cell *matHeaderCellDef mat-sort-header> {{{{'{entityTranslateString}.{translateString}' | translate}}}}  </mat-header-cell>\n" +
                                $"   <mat-cell *matCellDef=\"let row\" data-label=\"{f.Name}\">\n" +
                                $"       <mat-slide-toggle [(ngModel)]=\"row.{clientName}\" disabled> </mat-slide-toggle>\n" +
                                $"   </mat-cell>\n" +
                                $"</ng-container>\n";
                    if (f.PropertyType.ToString().Contains("DateTime"))
                        return $"<ng-container matColumnDef=\"{clientName}\">\n" +
                               $"    <mat-header-cell *matHeaderCellDef mat-sort-header> {{{{'{entityTranslateString}.{translateString}' | translate}}}} </mat-header-cell> \n" +
                               $"    <mat-cell *matCellDef=\"let row\" data-label=\"{f.Name}\"> {{{{row.{clientName} | date:'yyyy-MM-dd HH:mm:ss'}}}} </mat-cell> \n" +
                               $"</ng-container>\n";
                    else
                        return $"<ng-container matColumnDef=\"{clientName}\">\n" +
                               $"    <mat-header-cell *matHeaderCellDef mat-sort-header> {{{{'{entityTranslateString}.{translateString}' | translate}}}} </mat-header-cell> \n" +
                               $"    <mat-cell *matCellDef=\"let row\" data-label=\"{f.Name}\"> {{{{row.{clientName}}}}}</mat-cell> \n" +
                               $"</ng-container>\n";

                })
                .ToList();

            componentHTMLTemplateCopy = componentHTMLTemplateCopy.Replace("@TABLE_COLUMN_DEFINITOINS@", string.Join("\n", columnList));

            var filterList = Type.GetType(_appNameSpace + ".Models." + _entityName)
               .GetProperties()
               .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
               .Where(f => f.Name != "UpdatedDate" && f.Name != "UpdatedUser")
               .Select(f =>
               {
                   string translateString = f.Name.ToUpperInvariant();
                   string clientName = f.Name.Substring(0, 1).ToLowerInvariant() + f.Name.Substring(1, f.Name.Length - 1);

                   if (f.PropertyType.ToString().Contains("Boolean"))
                       return $"<mat-slide-toggle [(ngModel)]=\"filter.{clientName}\">{{{{'{entityTranslateString}.{translateString}'| translate}}}}</mat-slide-toggle>";
                   if (f.PropertyType.ToString().Contains("DateTime"))
                       return $"<div>\n" +
                               $"   <mat-form-field [style.width.%]=\"45\" appearance=\"legacy\">\n" +
                               $"       <mat-label>{{{{'{entityTranslateString}.{translateString}'| translate}}}}</mat-label> \n" +
                               $"       <input matInput [matDatepicker]=\"{clientName}picker\" [(ngModel)]=\"filter.{clientName}\">\n" +
                               $"       <mat-datepicker-toggle matSuffix [for]=\"{clientName}picker\"></mat-datepicker-toggle>\n" +
                               $"       <mat-datepicker #{clientName}picker></mat-datepicker>\n" +
                               $"   </mat-form-field>\n" +
                               $"   &nbsp;\n" +
                               $"   <mat-form-field [style.width.%]=\"45\" appearance=\"legacy\">\n" +
                               $"       <mat-label>{{{{'{entityTranslateString}.{translateString}'| translate}}}}</mat-label>\n" +
                               $"       <input matInput [matDatepicker]=\"{clientName}2picker\" [(ngModel)]=\"filter.{clientName}2\">\n" +
                               $"       <mat-datepicker-toggle matSuffix [for]=\"{clientName}2picker\"></mat-datepicker-toggle>\n" +
                               $"       <mat-datepicker #{clientName}2picker></mat-datepicker>\n" +
                               $"   </mat-form-field>\n" +
                               $"</div>";
                   else
                       return $"<mat-form-field [style.width.%]=\"100\" appearance=\"legacy\">\n" +
                                  $"    <mat-label>{{{{'{entityTranslateString}.{translateString}'| translate}}}}</mat-label>\n" +
                                  $"    <input matInput type=\"{(f.PropertyType.ToString() == "System.String" ? "text" : "number") }\" [(ngModel)]=\"filter.{clientName}\">\n" +
                                  $"</mat-form-field>";

               })
               .ToList();

            componentHTMLTemplateCopy = componentHTMLTemplateCopy.Replace("@FILTER_LIST@", string.Join("\n", filterList));

            return componentHTMLTemplateCopy;
        }
        public string CreateComponentTsClientCode()
        {
            string componentTSTemplateCopy = _componentTSTemplate;
            var entityTranslateString = _entityName.ToUpperInvariant();
            var entityLower = _entityName.ToLowerInvariant();
            var entitySmall = _entityName.Substring(0, 1).ToLowerInvariant() + _entityName.Substring(1, _entityName.Length - 1);

            componentTSTemplateCopy = componentTSTemplateCopy.Replace("@ENTITY_UPPER@", entityTranslateString);
            componentTSTemplateCopy = componentTSTemplateCopy.Replace("@ENTITY@", _entityName);
            componentTSTemplateCopy = componentTSTemplateCopy.Replace("@ENTITY_LOWER@", entityLower);
            componentTSTemplateCopy = componentTSTemplateCopy.Replace("@ENTITY_SMALL@", entitySmall);

            var newItemSetList = Type.GetType(_appNameSpace + ".Models." + _entityName)
                .GetProperties()
                .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
                .Select(f =>
                {
                    string translateString = f.Name.ToUpperInvariant();
                    string clientName = f.Name.Substring(0, 1).ToLowerInvariant() + f.Name.Substring(1, f.Name.Length - 1);
                    string itemString = $"emptyData.{clientName} = @Value@;";

                    if (f.PropertyType.ToString().Contains("Boolean"))
                    {
                        itemString = itemString.Replace("@Value@", "false");
                    }
                    else if (f.PropertyType.ToString().Contains("DateTime"))
                    {
                        itemString = itemString.Replace("@Value@", "new Date()");
                    }
                    else if (f.PropertyType.ToString().Contains("String"))
                    {
                        itemString = itemString.Replace("@Value@", "''");
                    }
                    else
                    {
                        itemString = itemString.Replace("@Value@", "0");
                    }

                    return itemString;

                })
                .ToList();

            componentTSTemplateCopy = componentTSTemplateCopy.Replace("@NEWITEM_FIELD_SET@", string.Join("\n", newItemSetList));



            var configList = Type.GetType(_appNameSpace + ".Models." + _entityName)
               .GetProperties()
               .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
               .Where(f => f.Name != "CreatedUser" && f.Name != "CreatedDate" && f.Name != "UpdatedUser" && f.Name != "UpdatedDate")
               .Select(f =>
               {
                   string translateString = f.Name.ToUpperInvariant();
                   string clientName = f.Name.Substring(0, 1).ToLowerInvariant() + f.Name.Substring(1, f.Name.Length - 1);
                   string showColumn = (f.Name == "CreatedDate" || f.Name == "CreatedUser") ? "false" : "true";
                   string configTemplate = "{ \n" +
                                  $"   prop: '{clientName}',\n" +
                                  $"   translate: '{translateString}',\n" +
                                  $"   showColumn: {showColumn},\n" +
                                  "   isExport: true,\n" +
                                  "   localText: ''\n" +
                                  "}";
                   return configTemplate;

               })
               .ToList();

            componentTSTemplateCopy = componentTSTemplateCopy.Replace("@DATACONFIG@", string.Join(",\n", configList));

            var entityDataFieldList = Type.GetType(_appNameSpace + ".Models." + _entityName)
             .GetProperties()
             .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
             .Where(f => f.Name != "CreatedUser" && f.Name != "CreatedDate" && f.Name != "UpdatedUser" && f.Name != "UpdatedDate")
             .Select(f =>
             {
                 string translateString = f.Name.ToUpperInvariant();
                 string clientName = f.Name.Substring(0, 1).ToLowerInvariant() + f.Name.Substring(1, f.Name.Length - 1);

                 string itemString = $"{clientName} : @TYPE@;";

                 if (f.PropertyType.ToString().Contains("Boolean"))
                 {
                     itemString = itemString.Replace("@TYPE@", "boolean");
                 }
                 else if (f.PropertyType.ToString().Contains("DateTime"))
                 {
                     itemString = itemString.Replace("@TYPE@", "Date");
                 }
                 else if (f.PropertyType.ToString().Contains("String"))
                 {
                     itemString = itemString.Replace("@TYPE@", "string");
                 }
                 else
                 {
                     itemString = itemString.Replace("@TYPE@", "number");
                 }

                 return itemString;


             })
             .ToList();


            componentTSTemplateCopy = componentTSTemplateCopy.Replace("@ENTITYDATAFIELDS@", string.Join("\n", entityDataFieldList));


            var entityFilterFieldList = Type.GetType(_appNameSpace + ".Models." + _entityName)
           .GetProperties()
           .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
           .Where(f => f.Name != "CreatedUser" && f.Name != "CreatedDate" && f.Name != "UpdatedUser" && f.Name != "UpdatedDate")
           .Select(f =>
           {
               string translateString = f.Name.ToUpperInvariant();

               string clientName = f.Name.Substring(0, 1).ToLowerInvariant() + f.Name.Substring(1, f.Name.Length - 1);

               string itemString = $"{clientName} : @TYPE@;";

               if (f.PropertyType.ToString().Contains("Boolean"))
               {
                   itemString = itemString.Replace("@TYPE@", "boolean");
               }
               else if (f.PropertyType.ToString().Contains("DateTime"))
               {
                   itemString = itemString.Replace("@TYPE@", "Date");
                   itemString += $"\n{clientName}2 : Date";
               }
               else if (f.PropertyType.ToString().Contains("String"))
               {
                   itemString = itemString.Replace("@TYPE@", "string");
               }
               else
               {
                   itemString = itemString.Replace("@TYPE@", "number");
               }

               return itemString;
           })
           .ToList();


            componentTSTemplateCopy = componentTSTemplateCopy.Replace("@ENTITYFILTERFIELDS@", string.Join("\n", entityFilterFieldList));

            return componentTSTemplateCopy;
        }
        public string CreateDialogComponentHTMLClientCode()
        {
            string dialogComponentHTMLTemplateCopy = _dialogComponentHTMLTemplate;
            var entityTranslateString = _entityName.ToUpperInvariant();
            var entityLower = _entityName.ToLowerInvariant();
            var entitySmall = _entityName.Substring(0, 1).ToLowerInvariant() + _entityName.Substring(1, _entityName.Length - 1);
            var primaryClient = _primaryKey.Substring(0, 1).ToLowerInvariant() + _primaryKey.Substring(1, _primaryKey.Length - 1);
            dialogComponentHTMLTemplateCopy = dialogComponentHTMLTemplateCopy.Replace("@ENTITYUPPER@", entityTranslateString);
            dialogComponentHTMLTemplateCopy = dialogComponentHTMLTemplateCopy.Replace("@ENTITY@", entityTranslateString);
            dialogComponentHTMLTemplateCopy = dialogComponentHTMLTemplateCopy.Replace("@ENTITY_LOWER@", entityLower);
            dialogComponentHTMLTemplateCopy = dialogComponentHTMLTemplateCopy.Replace("@ENTITY_SMALL@", entitySmall);
            dialogComponentHTMLTemplateCopy = dialogComponentHTMLTemplateCopy.Replace("@PRIMARYKEY@", primaryClient);


            var matFormFieldList = Type.GetType(_appNameSpace + ".Models." + _entityName)
          .GetProperties()
          .Where(f => !f.PropertyType.ToString().Contains("System.Collections.Generic.ICollection") && !f.PropertyType.ToString().Contains(".Models."))
          .Where(f => f.Name != "CreatedUser" && f.Name != "CreatedDate" && f.Name != "UpdatedUser" && f.Name != "UpdatedDate" && f.Name!=_primaryKey)
          .Select(f =>
          {
              string translateString = f.Name.ToUpperInvariant();
              string clientName = f.Name.Substring(0, 1).ToLowerInvariant() + f.Name.Substring(1, f.Name.Length - 1);

              string itemString;

              if (f.PropertyType.ToString().Contains("Boolean"))
              {
                  itemString = "<mat-form-field [style.width.%]=\"100\" appearance=\"legacy\">\n" +
                                $"   <mat-slide-toggle [(ngModel)]=\"row.{clientName}\"> </mat-slide-toggle>\n" +
                                $"   <span style=\"margin-left:10px;\">{{{{ '{entityTranslateString}.{translateString}' | translate }}}}</span>\n" +
                                "   <textarea matInput style=\"display:none;\"></textarea>\n" +
                                "</mat-form-field>";
              }
              else if (f.PropertyType.ToString().Contains("DateTime"))
              {
                  itemString = "<mat-form-field  [style.width.%]=\"100\" appearance=\"legacy\">\n" +
                               $"    <mat-label>{{{{ '{entityTranslateString}.{translateString}' | translate }}}}</mat-label>\n" +
                               $"    <input matInput [matDatepicker]=\"{clientName}Picker\" [(ngModel)]=\"row.{clientName}\">\n" +
                               $"     <mat-datepicker-toggle matSuffix [for]=\"{clientName}Picker\"></mat-datepicker-toggle>\n" +
                               $"    <mat-datepicker #{clientName}Picker></mat-datepicker>\n" +
                               " </mat-form-field>";
              }
              else if (f.PropertyType.ToString().Contains("String"))
              {
                  itemString = "<mat-form-field [style.width.%]=\"100\" appearance=\"legacy\">\n" +
                               $"    <mat-label>{{{{ '{entityTranslateString}.{translateString}' | translate }}}}</mat-label>\n" +
                               $"   <input type=\"text\" matInput [(ngModel)]=\"row.{clientName}\"> \n" +
                               "</mat-form-field>";
              }
              else
              {
                  itemString = "<mat-form-field [style.width.%]=\"100\" appearance=\"legacy\">\n" +
                               $"    <mat-label>{{{{ '{entityTranslateString}.{translateString}' | translate }}}}</mat-label>\n" +
                               $"   <input type=\"number\" matInput [(ngModel)]=\"row.{clientName}\"> \n" +
                               "</mat-form-field>";
              }

              return itemString;
          })
          .ToList();

            dialogComponentHTMLTemplateCopy = dialogComponentHTMLTemplateCopy.Replace("@MATFORMFIELDS@", string.Join("\n", matFormFieldList));

            return dialogComponentHTMLTemplateCopy;
        }
        public string CreateDialogComponentTSClientCode()
        {
            string dialogComponentTSTemplateCopy = _dialogComponentTSTemplate;
            var entityTranslateString = _entityName.ToUpperInvariant();
            var entityLower = _entityName.ToLowerInvariant();
            var entitySmall = _entityName.Substring(0, 1).ToLowerInvariant() + _entityName.Substring(1, _entityName.Length - 1);

            dialogComponentTSTemplateCopy = dialogComponentTSTemplateCopy.Replace("@ENTITY_UPPER@", entityTranslateString);
            dialogComponentTSTemplateCopy = dialogComponentTSTemplateCopy.Replace("@ENTITY@", _entityName);
            dialogComponentTSTemplateCopy = dialogComponentTSTemplateCopy.Replace("@ENTITY_LOWER@", entityLower);
            dialogComponentTSTemplateCopy = dialogComponentTSTemplateCopy.Replace("@ENTITY_SMALL@", entitySmall);

            return dialogComponentTSTemplateCopy;
        }
        public string CreatServiceTSClientCode()
        {
            string _serviceTSTemplateCopy = _serviceTSTemplate;
            var entityTranslateString = _entityName.ToUpperInvariant();
            var entityLower = _entityName.ToLowerInvariant();
            var entitySmall = _entityName.Substring(0, 1).ToLowerInvariant() + _entityName.Substring(1, _entityName.Length - 1);

            _serviceTSTemplateCopy = _serviceTSTemplateCopy.Replace("@ENTITY_UPPER@", entityTranslateString);
            _serviceTSTemplateCopy = _serviceTSTemplateCopy.Replace("@ENTITY@", _entityName);
            _serviceTSTemplateCopy = _serviceTSTemplateCopy.Replace("@ENTITY_LOWER@", entityLower);
            _serviceTSTemplateCopy = _serviceTSTemplateCopy.Replace("@ENTITY_SMALL@", entitySmall);

            return _serviceTSTemplateCopy;
        }
    }
}
