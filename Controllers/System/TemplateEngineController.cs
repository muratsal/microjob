using NanoGo.AppTemplates;
using NanoGo.AppTemplates.AngularTemplates;
using NanoGo.Services.System;
using NanoGo.Shared;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NanoGo.Controllers.System
{
    [Route("api/[controller]")]
    [ApiController]
    public class TemplateEngineController : ControllerBase
    {
        private CodeCreator _codeCreator;
        private AngularCodeCreator _angularCodeCreator;
        private readonly SystemLogService _logger;
        private IWebHostEnvironment _hostEnvironment;

        public TemplateEngineController(IWebHostEnvironment environment)
        {
            _hostEnvironment = environment;
            _logger = new SystemLogService();
            _codeCreator = new CodeCreator()
                .SetAppNamepace("NanoGo")
                .SetContextName("NanoGoContext");
            _angularCodeCreator = new AngularCodeCreator()
              .SetAppNamepace("NanoGo")
              .SetContextName("NanoGoContext");
        }


        [HttpGet]
        [Route("CreateBackendTemplate")]
        public ReturnInfo CreateBackendTemplate(string menu, string entity, string primaryKey)
        {

            ReturnInfo returnInfo = new ReturnInfo();

            try
            {
                var serviceTemplate = global::System.IO.File.ReadAllText(Path.Combine(_hostEnvironment.ContentRootPath, "AppTemplates/BE_ServiceTemplate.txt"));
                var serviceCode = _codeCreator
                                    .SetEntityName(entity)
                                    .SetPrimaryKey(primaryKey)
                                    .SetServiceTemplate(serviceTemplate)
                                    .CreateServiceCode();
                string createdFileName = "Services/" + menu + "/" + entity + "Service.cs";
                global::System.IO.File.WriteAllText(Path.Combine(_hostEnvironment.ContentRootPath, createdFileName), serviceCode);
                returnInfo.Data = createdFileName + " created";

                var contollerTemplate = global::System.IO.File.ReadAllText(Path.Combine(_hostEnvironment.ContentRootPath, "AppTemplates/BE_ControllerTemplate.txt"));
                var contollerCode = _codeCreator
                                    .SetEntityName(entity)
                                    .SetPrimaryKey(primaryKey)
                                    .SetServiceTemplate(contollerTemplate)
                                    .CreateContollerCode();
                string createdContollerFileName = "Controllers/" + menu + "/" + entity + "Contoller.cs";
                global::System.IO.File.WriteAllText(Path.Combine(_hostEnvironment.ContentRootPath, createdContollerFileName), contollerCode);

                returnInfo.IsSuccess = true;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("TemplateEngine.CreateBackendTemplate", ex.ToString(), 1);
            }

            return returnInfo;
        }

        [HttpGet]
        [Route("CreateFrontEndTemplate")]
        public ReturnInfo CreateFrontEndTemplate(string menu, string entity, string primaryKey)
        {
            ReturnInfo returnInfo = new ReturnInfo();
            try
            {
                string directoryMenu = "wwwroot/app/src/app/views/" + menu ;

                if (!Directory.Exists(Path.Combine(_hostEnvironment.ContentRootPath, directoryMenu)))
                {
                    Directory.CreateDirectory(Path.Combine(_hostEnvironment.ContentRootPath, directoryMenu));
                }

                if (!Directory.Exists(Path.Combine(_hostEnvironment.ContentRootPath, directoryMenu + "/" + entity)))
                {
                    Directory.CreateDirectory(Path.Combine(_hostEnvironment.ContentRootPath, directoryMenu + "/" + entity));
                }

                var componentHTMLTemplate = global::System.IO.File.ReadAllText(Path.Combine(_hostEnvironment.ContentRootPath, "AppTemplates/AngularTemplates/FE_Component_Html.txt"));
                var componentHTMLCode = _angularCodeCreator
                                    .SetEntityName(entity)
                                    .SetPrimaryKey(primaryKey)
                                    .SetComponentHTMLTemplate(componentHTMLTemplate)
                                    .CreateComponentHTMLClientCode();
                string componentHTMLFileName = "wwwroot/app/src/app/views/" + menu + "/" + entity + "/" + entity.ToLowerInvariant() + ".component.html";
                global::System.IO.File.WriteAllText(Path.Combine(_hostEnvironment.ContentRootPath, componentHTMLFileName), componentHTMLCode);

                var componentTSTemplate = global::System.IO.File.ReadAllText(Path.Combine(_hostEnvironment.ContentRootPath, "AppTemplates/AngularTemplates/FE_Component_Ts.txt"));
                var componentTSCode = _angularCodeCreator
                                    .SetEntityName(entity)
                                    .SetPrimaryKey(primaryKey)
                                    .SetComponentTSTemplate(componentTSTemplate)
                                    .CreateComponentTsClientCode();

                string componentTSFileName = "wwwroot/app/src/app/views/" + menu + "/" + entity + "/" + entity.ToLowerInvariant() + ".component.ts";
                global::System.IO.File.WriteAllText(Path.Combine(_hostEnvironment.ContentRootPath, componentTSFileName), componentTSCode);

                var serviceTSTemplate = global::System.IO.File.ReadAllText(Path.Combine(_hostEnvironment.ContentRootPath, "AppTemplates/AngularTemplates/FE_Service_Ts.txt"));
                var serviceTSCode = _angularCodeCreator
                                    .SetEntityName(entity)
                                    .SetPrimaryKey(primaryKey)
                                    .SetServiceTSTemplate(serviceTSTemplate)
                                    .CreatServiceTSClientCode();

                string serviceTSFileName = "wwwroot/app/src/app/views/" + menu + "/" + entity + "/" + entity.ToLowerInvariant() + ".service.ts";
                global::System.IO.File.WriteAllText(Path.Combine(_hostEnvironment.ContentRootPath, serviceTSFileName), serviceTSCode);

                var dialogComponentHtmlTempalte = global::System.IO.File.ReadAllText(Path.Combine(_hostEnvironment.ContentRootPath, "AppTemplates/AngularTemplates/FE_Dialog_Component_Html.txt"));
                var dialogComponentHtmlCode = _angularCodeCreator
                                    .SetEntityName(entity)
                                    .SetPrimaryKey(primaryKey)
                                    .SetDialogComponentHTMLTemplate(dialogComponentHtmlTempalte)
                                    .CreateDialogComponentHTMLClientCode();

                string dialogComponentHtmlFileName = "wwwroot/app/src/app/views/" + menu + "/" + entity + "/" + entity.ToLowerInvariant() + "-dialog.component.html";
                global::System.IO.File.WriteAllText(Path.Combine(_hostEnvironment.ContentRootPath, dialogComponentHtmlFileName), dialogComponentHtmlCode);


                var dialogComponentTSTempalte = global::System.IO.File.ReadAllText(Path.Combine(_hostEnvironment.ContentRootPath, "AppTemplates/AngularTemplates/FE_Dialog_Component_Ts.txt"));
                var dialogComponentTSCode = _angularCodeCreator
                                    .SetEntityName(entity)
                                    .SetPrimaryKey(primaryKey)
                                    .SetDialogComponentTSTemplate(dialogComponentTSTempalte)
                                    .CreateDialogComponentTSClientCode();

                string dialogComponentTSFileName = "wwwroot/app/src/app/views/" + menu + "/" + entity + "/" + entity.ToLowerInvariant() + "-dialog.component.ts";
                global::System.IO.File.WriteAllText(Path.Combine(_hostEnvironment.ContentRootPath, dialogComponentTSFileName), dialogComponentTSCode);

                string componentSCSSFileName = "wwwroot/app/src/app/views/" + menu + "/" + entity + "/" + entity.ToLowerInvariant() + ".component.scss";
                global::System.IO.File.WriteAllText(Path.Combine(_hostEnvironment.ContentRootPath, componentSCSSFileName), "");


                returnInfo.IsSuccess = true;
            }
            catch (Exception ex)
            {
                returnInfo.IsSuccess = false;
                returnInfo.ErrorMessage = ex.Message;
                _logger.AddLog("TemplateEngine.CreateBackendTemplate", ex.ToString(), 1);
            }

            return returnInfo;
        }
    }
}
