using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyRealWordUnitTest.Web.Controllers;
using UdemyRealWordUnitTest.Web.Model;
using UdemyRealWordUnitTest.Web.Repository;
using Xunit;

namespace UdemyRealWordUnitTest.Test
{
    public class ProductControllerTest
    {
        private readonly Mock<IRepository<Product>>? _mockRepo;
        private readonly ProductsController _controller;
        private List<Product> _products;

        public ProductControllerTest()
        {
            _mockRepo = new Mock<IRepository<Product>>();
            _controller = new ProductsController(_mockRepo.Object);
            _products = new List<Product>()
            {
                new Product
                {
                    Id=1,
                    Name="pencil",
                    Price=3,
                    Stock=100,
                    Color="Blue"
                },
                new Product
                {
                    Id=2,
                    Name="Book",
                    Price=4,
                    Stock=200,
                    Color="red"
                }
            };
        }

        [Fact]
        public async void Index_ActionExucute_ReturnView()
        {
            var result = await _controller.Index();
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void Details_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Details(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void Details_InValid_ReturnNotFound()
        {
            Product p = null;
            _mockRepo.Setup(x => x.GetByIdAsync(0)).ReturnsAsync(p);

            var result = await _controller.Details(0);
            var redirect = Assert.IsType<NotFoundResult>(result);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(1)]
        public async void Details_ValidId_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Details(productId);
            var viewResult = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResult.Model);

            Assert.Equal(product.Id, resultProduct.Id);
            Assert.Equal(product.Name, resultProduct.Name);
        }

        [Fact]
        public void Create_ActionExecute_ReturnView()
        {
            var result = _controller.Create();
            var redirect = Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public async void CreatePost_InValidModelState_ReturnView()
        {
            _controller.ModelState.AddModelError("name", "name alani gereklidir");
            var result = await _controller.Create(_products.First());
            var viewResult = Assert.IsType<ViewResult>(result);
            Assert.IsType<Product>(viewResult.Model);
        }

        [Fact]
        public async void CreatePost_ValidModelState_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Create(_products.First());
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Fact]
        public async void CreatePost_ValidModelState_CerateMethodExecutes()
        {
            Product NewProduct = null;
            _mockRepo.Setup(repo => repo.CreateAsync(It.IsAny<Product>
                ())).Callback<Product>(x => NewProduct = x);

            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Once);

            Assert.Equal(_products.First().Id, NewProduct.Id);
        }

        [Fact]
        public async void CreatePost_InvalidModelState_NeverCreateExecute()
        {
            _controller.ModelState.AddModelError("name", "error");
            var result = await _controller.Create(_products.First());
            _mockRepo.Verify(repo => repo.CreateAsync(It.IsAny<Product>()), Times.Never);
        }

        [Fact]
        public async void Edit_IdIsNull_ReturnRedirectToIndexAction()
        {
            var result = await _controller.Edit(null);
            var redirect = Assert.IsType<RedirectToActionResult>(result);
            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(3)]
        public async void Edit_IdInValid_ReturnNotFound(int productId)
        {
            Product p = null;
            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(p);
            var reuslt = await _controller.Edit(productId);
            var redirect = Assert.IsType<NotFoundResult>(reuslt);
            Assert.Equal<int>(404, redirect.StatusCode);
        }

        [Theory]
        [InlineData(2)]
        public async void Edit_ActionExecute_ReturnProduct(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Edit(productId);
            var viewResul = Assert.IsType<ViewResult>(result);
            var resultProduct = Assert.IsAssignableFrom<Product>(viewResul.Model);

            Assert.Equal(product.Id, resultProduct.Id);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPost_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            var result = await _controller.Edit(2, _products.First(x => x.Id == productId));
            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPost_InValidModelState_ReturnView(int productId)
        {
            _controller.ModelState.AddModelError("Name", "Error");

            var result = await _controller.Edit(productId, _products.First(x => x.Id == productId));
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsType<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPost_ValidModelState_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.Edit(productId, _products.First(x => x.Id == productId));
            var redirect = Assert.IsType<RedirectToActionResult>(result);

            Assert.Equal("Index", redirect.ActionName);
        }

        [Theory]
        [InlineData(1)]
        public async void EditPost_ValidModelState_UpdateMethodExecute(int productId)
        {
            Product product = _products.First(p => p.Id == productId);
            _mockRepo.Setup(repo => repo.UpdateAsync(product));
            await _controller.Edit(productId, product);
            _mockRepo.Verify(repo => repo.UpdateAsync(It.IsAny<Product>()), Times.Once);
        }

        [Fact]
        public async void Delete_IdIsNull_ReturnNotFound()
        {
            var result = await _controller.Delete(null);
            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(3)]
        public async void Delete_IdIsNotEqualProduct_ReturnNotFound(int productId)
        {
            Product product = null;
            _mockRepo.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Delete(productId);
            var redirect = Assert.IsType<NotFoundResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void Delete_ActionExecute_ReturnView(int productId)
        {
            var product = _products.First(x => x.Id == productId);
            _mockRepo.Setup(repo => repo.GetByIdAsync(productId)).ReturnsAsync(product);

            var result = await _controller.Delete(productId);
            var viewResult = Assert.IsType<ViewResult>(result);

            Assert.IsAssignableFrom<Product>(viewResult.Model);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfired_ActionExecutes_ReturnRedirectToIndexAction(int productId)
        {
            var result = await _controller.DeleteConfirmed(productId);
            Assert.IsType<RedirectToActionResult>(result);
        }

        [Theory]
        [InlineData(1)]
        public async void DeleteConfired_ActionExecutes_DeleteMethodExecute(int productId)
        {
            var product = _products.First(x => x.Id == productId);

            _mockRepo.Setup(repo => repo.DeleteAsync(product));
            await _controller.DeleteConfirmed(productId);
            _mockRepo.Verify(repo => repo.DeleteAsync(It.IsAny<Product>()), Times.Once);
        }
    }
}
