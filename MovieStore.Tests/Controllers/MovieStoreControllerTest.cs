using System;
using System.Web.Mvc;
using System.Net;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MovieStore.Controllers;
using MovieStore.Models;
using Moq;
using System.Data.Entity;

namespace MovieStore.Tests.Controllers
{
    [TestClass]
    public class MovieStoreControllerTest
    {
        [TestMethod]
        public void IndexTest()
        {
            MoviesController controller = new MoviesController();
            ViewResult result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MovieStore_ListOfMovies()
        {
            MoviesController controller = new MoviesController();
            var result = controller.ListOfMovies();
            Assert.AreEqual("Terminator 1", result[0].Title);
            Assert.AreEqual("Terminator 2", result[1].Title);
            Assert.AreEqual("Terminator 3", result[2].Title);
        }

        [TestMethod]
        public void MovieStore_IndexRedirect_Success()
        {
            MoviesController controller = new MoviesController();
            var result = controller.IndexRedirect(1) as RedirectToRouteResult;
            Assert.AreEqual("Create", result.RouteValues["action"]);
            Assert.AreEqual("HomeController", result.RouteValues["controller"]);
        }

        [TestMethod]
        public void MovieStore_IndexRedirect_BadRequest()
        {
            MoviesController controller = new MoviesController();
            var result = controller.IndexRedirect(0) as HttpStatusCodeResult;
            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode) result.StatusCode);
        }

        [TestMethod]
        public void MovieStore_ListFromDb()
        {
            var list = new List<Movie>
            {
                new Movie() {MovieId=1, Title="Jaws"},
                new Movie() {MovieId=2, Title="Jurrasic Park"},
                new Movie() {MovieId=3, Title="Titanic"}
            }.AsQueryable();

            Mock<MovieStoreDbContext> mockContext = new Mock<MovieStoreDbContext>();
            Mock<DbSet<Movie>> mockSet = new Mock<DbSet<Movie>>();

            mockSet.As<IQueryable<Movie>>().Setup(m=>m.GetEnumerator()).Returns(list.GetEnumerator());
            mockSet.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(list.Provider);
            mockSet.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(list.ElementType);

            mockContext.Setup(db => db.Movies).Returns(mockSet.Object);

            MoviesController controller = new MoviesController(mockContext.Object);
            ViewResult result = controller.ListFromDb() as ViewResult;
            List<Movie> resultModel = result.Model as List<Movie>;

            Assert.IsNotNull(result);
            Assert.AreEqual("Jaws", resultModel[0].Title);
            Assert.AreEqual("Jurrasic Park", resultModel[1].Title);
            Assert.AreEqual("Titanic", resultModel[2].Title);
        }

        [TestMethod]
        public void MovieStore_Details_Success()
        {
            var list = new List<Movie>
            {
                new Movie() {MovieId=1, Title="Jaws"},
                new Movie() {MovieId=2, Title="Jurrasic Park"},
                new Movie() {MovieId=3, Title="Titanic"}
            }.AsQueryable();

            Mock<MovieStoreDbContext> mockContext = new Mock<MovieStoreDbContext>();
            Mock<DbSet<Movie>> mockSet = new Mock<DbSet<Movie>>();

            mockSet.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(list.GetEnumerator());
            mockSet.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(list.Provider);
            mockSet.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(list.ElementType);
            mockSet.Setup(m => m.Find(It.IsAny<Object>())).Returns(list.First());

            mockContext.Setup(db => db.Movies).Returns(mockSet.Object);

            MoviesController controller = new MoviesController(mockContext.Object);
            ViewResult result = controller.Details(1) as ViewResult;
           
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void MovieStore_Details_No_Id()
        {
            var list = new List<Movie>
            {
                new Movie() {MovieId=1, Title="Jaws"},
                new Movie() {MovieId=2, Title="Jurrasic Park"},
                new Movie() {MovieId=3, Title="Titanic"}
            }.AsQueryable();

            Mock<MovieStoreDbContext> mockContext = new Mock<MovieStoreDbContext>();
            Mock<DbSet<Movie>> mockSet = new Mock<DbSet<Movie>>();

            mockSet.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(list.GetEnumerator());
            mockSet.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(list.Provider);
            mockSet.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(list.ElementType);
            mockSet.Setup(m => m.Find(It.IsAny<Object>())).Returns(list.First());

            mockContext.Setup(db => db.Movies).Returns(mockSet.Object);

            MoviesController controller = new MoviesController(mockContext.Object);
            HttpStatusCodeResult result = controller.Details(null) as HttpStatusCodeResult;

            Assert.AreEqual(HttpStatusCode.BadRequest, (HttpStatusCode)result.StatusCode);
        }

        [TestMethod]
        public void MovieStore_Details_MovieNull()
        {
            var list = new List<Movie>
            {
                new Movie() {MovieId=1, Title="Jaws"},
                new Movie() {MovieId=2, Title="Jurrasic Park"},
                new Movie() {MovieId=3, Title="Titanic"}
            }.AsQueryable();

            Mock<MovieStoreDbContext> mockContext = new Mock<MovieStoreDbContext>();
            Mock<DbSet<Movie>> mockSet = new Mock<DbSet<Movie>>();

            Movie movie = null;
            mockSet.As<IQueryable<Movie>>().Setup(m => m.GetEnumerator()).Returns(list.GetEnumerator());
            mockSet.As<IQueryable<Movie>>().Setup(m => m.Provider).Returns(list.Provider);
            mockSet.As<IQueryable<Movie>>().Setup(m => m.ElementType).Returns(list.ElementType);
            mockSet.Setup(m => m.Find(It.IsAny<Object>())).Returns(movie);

            mockContext.Setup(db => db.Movies).Returns(mockSet.Object);

            MoviesController controller = new MoviesController(mockContext.Object);
            HttpStatusCodeResult result = controller.Details(1) as HttpStatusCodeResult;

            Assert.AreEqual(HttpStatusCode.NotFound, (HttpStatusCode)result.StatusCode);
        }
    }
}
