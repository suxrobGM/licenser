using Licenser.Server.Domain.Entities;
using Licenser.WebApi.Controllers;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace Licenser.UnitTests
{
    public class ApplicationRolesControllerTests
    {
        private readonly ApplicationRolesController _controller;

        public ApplicationRolesControllerTests()
        {
            var userManagerMock = new Mock<UserManager<User>>();
            var roleManagerMock = new Mock<RoleManager<Role>>();
            _controller = new ApplicationRolesController(userManagerMock.Object, roleManagerMock.Object);
            
            ConfigureUserManager(userManagerMock);
            ConfigureRoleManager(roleManagerMock);
        }

        private void ConfigureUserManager(Mock<UserManager<User>> userManagerMock)
        {
            
        }
        
        private void ConfigureRoleManager(Mock<RoleManager<Role>> roleManagerMock)
        {
            
        }

        [Fact]
        public void GetRoles_ReturnsAllItems()
        {
            
        }
    }
}