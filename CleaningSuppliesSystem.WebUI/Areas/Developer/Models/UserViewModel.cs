using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;

namespace CleaningSuppliesSystem.WebUI.Areas.Developer.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string NameSurname { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserName  { get; set; }
        public string Email  { get; set; }
        public DateTime CreatedAt  { get; set; }
        public bool IsActive { get; set; }
        public List<string> Role { get; set; }
        public List<string> AllRoles { get; set; }
    }
}