using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web.Http;
using System.Web.Mvc;
using InsideReporting.Controllers;
using InsideReporting.Helpers;
using InsideReporting.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using AuthorizeAttribute = System.Web.Mvc.AuthorizeAttribute;

namespace InsideReporting.Tests.Controllers
{
    [TestClass]
    public class ControllerPolicyTest
    {
        [TestMethod]
        public void AllInsideControllersAreAthorizedExpectLogin()
        {

            var loadedAssembly = Assembly.Load("InsideReporting");
            var allTypes = loadedAssembly.GetTypes();
            var controllers = allTypes.Where(t => typeof(Controller).IsAssignableFrom(t));

            int numFails = 0;
            var failedMethods = new List<string>();
            foreach (var controller in controllers)
            {
                var isAdminController = false;

                var methods = GetMethodsOfReturnType(controller, typeof(ActionResult));

                var controllerAttributes = controller.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                foreach (var attribute in controllerAttributes)
                {
                    var haveConsultant = ((AuthorizeAttribute)attribute).Roles.Contains("consultant");
                    var doesNotHaveClient = ((AuthorizeAttribute) attribute).Roles.Contains("client");

                    isAdminController = haveConsultant && !doesNotHaveClient;
                }

                if (isAdminController)
                {
                    continue;
                }

                foreach (var method in methods)
                {
                    var isAdminMethod = false;

                    var methodAttributes = method.GetCustomAttributes(typeof(AuthorizeAttribute), true);
                    foreach (var attribute in methodAttributes)
                    {
                        var haveConsultant = ((AuthorizeAttribute)attribute).Roles.Contains("consultant");
                        var haveClient = ((AuthorizeAttribute)attribute).Roles.Contains("client");

                        isAdminMethod = haveConsultant && !haveClient;
                    }

                    if (isAdminMethod)
                    {
                        continue;
                    }


                    Expression<Action<AccountController>> logonGetAction = c => c.LogOn("url");
                    var islogonGetAction = method == ((MethodCallExpression)logonGetAction.Body).Method;

                    Expression<Action<AccountController>> logonPostAction = c => c.LogOn(new LogOnModel());
                    var isLogonPostAction = method == ((MethodCallExpression)logonPostAction.Body).Method;

                    Expression<Action<AccountController>> logofAction = c => c.LogOff();
                    var islogofAction = method == ((MethodCallExpression)logofAction.Body).Method;
                    
                    bool hasAutorizeAttribute = method.GetCustomAttributes(typeof(AuthorizeClientAccessAttribute), true).Length > 0;
                    bool hasClientId = method.GetParameters().AsQueryable().Any(par => par.Name == "clientId");
                    if (isLogonPostAction || islogonGetAction || islogofAction)
                    {
                        Assert.IsFalse(hasAutorizeAttribute, "Logon måste vara utan autentisering");
                    }
                    else if (!hasAutorizeAttribute && hasClientId)
                    {
                        numFails++;
                        failedMethods.Add(controller.Name + " - " + method.Name);
                    }
                }
            }

            Assert.IsTrue(numFails == 0, "Missing authentication for:  " + String.Join(", ", failedMethods));
        }


        [TestMethod]
        public void AllInsideApiControllersAreAthorized()
        {

            var loadedAssembly = Assembly.Load("InsideReporting");
            var allTypes = loadedAssembly.GetTypes();
            var controllers = allTypes.Where(t => typeof(ApiController).IsAssignableFrom(t));

            int numFails = 0;
            var failedMethods = new List<string>();
            foreach (var controller in controllers)
            {
                var methods = GetMethodsOfReturnType(controller, typeof(IHttpActionResult));

                foreach (var method in methods)
                {
                    bool hasAutorizeAttribute = method.GetCustomAttributes(typeof(AuthorizeClientAPIAccessAttribute), true).Length > 0;
                    bool hasClientId = method.GetParameters().AsQueryable().Any(par => par.Name == "clientId");
                    bool hasAccessToken = method.GetParameters().AsQueryable().Any(par => par.Name == "accessToken");

                    if (!hasClientId)
                    {
                        // ok
                    }
                    else if (hasClientId && (hasAutorizeAttribute || hasAccessToken))
                    {
                        //ok
                    }
                    else
                    {
                        numFails++;
                        failedMethods.Add(controller.Name + " - " + method.Name);
                    }
                }
            }

            Assert.IsTrue(numFails == 0, "Missing authentication for: " + String.Join(", ", failedMethods));
        }


        private IEnumerable<MethodInfo> GetMethodsOfReturnType(Type cls, Type ret)
        {
            // Did you really mean to prohibit public methods? I assume not
            var methods = cls.GetMethods(BindingFlags.NonPublic |
                                         BindingFlags.Public |
                                         BindingFlags.Instance);
            var retMethods = methods.Where(m => m.ReturnType == ret);
            return retMethods;
        }
    }
}
