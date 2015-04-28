﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using Castle.Windsor;

namespace Quilt4.Web
{
    public class CastleControllerFactory : DefaultControllerFactory
    {
        private IWindsorContainer Container { get; set; }

        public CastleControllerFactory(IWindsorContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container");
            }

            Container = container;
        }

        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                return null;
            }

            // Retrieve the requested controller from Castle
            return Container.Resolve(controllerType) as IController;
        }

        public override void ReleaseController(IController controller)
        {
            // If controller implements IDisposable, clean it up responsibly
            var disposableController = controller as IDisposable;
            if (disposableController != null)
            {
                disposableController.Dispose();
            }

            // Inform Castle that the controller is no longer required
            Container.Release(controller);
        }
    }
}