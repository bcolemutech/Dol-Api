namespace DolApiTest.Controllers
{
    using System;
    using DolApi.Controllers;
    using Xunit;

    public class AreaControllerTest
    {
        private readonly AreaController _sut;

        public AreaControllerTest()
        {
            _sut = new AreaController();
        }

        [Fact]
        public void PutGetsExistingRecordIfExistsAndReturnsOkObject()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void PutReturnsNotFoundIfResourceDoesNotExist()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void PutUpdatesResource()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void PutValidatesRecordAndReturnBadRequestIfInvalid()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void GetReturnsOkObjectIfFound()
        {
            throw new NotImplementedException();
        }

        [Fact]
        public void GetReturnsNotFoundIfResourceDoesNotExist()
        {
            throw new NotImplementedException();
        }
    }
}