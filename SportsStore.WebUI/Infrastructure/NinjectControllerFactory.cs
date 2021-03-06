﻿using System;
using System.Web.Mvc;
using System.Web.Routing;
using Ninject;
using SportsStore.Domain.Entities;
using SportsStore.Domain.Abstract;
using System.Collections.Generic;
using System.Linq;
using Moq;
using SportsStore.Domain.Concrete;
using System.Configuration;

namespace SportsStore.WebUI.Infrastructure
{
    public class NinjectControllerFactory : DefaultControllerFactory
    {
        private IKernel ninjectKernel;

        public NinjectControllerFactory()
        {
            ninjectKernel = new StandardKernel();
            AddBindings();
        }

        protected override IController GetControllerInstance(RequestContext requestContext,
            Type controllerType)
        {
            return controllerType == null
                ? null
                : (IController)ninjectKernel.Get(controllerType);
        }

        // Production Code
        // 
        private void AddBindings()
        {
            ninjectKernel.Bind<IProductsRepository>().To<EFProductRepository>();

            EmailSettings emailSetting = new EmailSettings
            {
                WriteAsFile = bool.Parse(ConfigurationManager
                    .AppSettings["EmailWriteAsFile"] ?? "false")
            };

            ninjectKernel.Bind<IOrderProcessor>()
                .To<EmailOrderProcessor>()
                .WithConstructorArgument("setting", emailSetting);
        }

        // Test Code - Would not show in production
        //
        //private void AddBindings()
        //{
        //    Mock<IProductsRepository> mock = new Mock<IProductsRepository>();
        //    mock.Setup(m => m.Products).Returns(new List<Product>
        //    {
        //        new Product { Name = "Football", Price = 25 },
        //        new Product { Name = "Surf board", Price = 179 },
        //        new Product { Name = "Running shoes", Price = 95 }
        //    }.AsQueryable());

        //    ninjectKernel.Bind<IProductsRepository>().ToConstant(mock.Object);
        //}
    }
}