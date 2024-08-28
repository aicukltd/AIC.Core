namespace AIC.Core.Identity.Models.Contracts;

using AIC.Core.Identity.Models.Implementations;

public interface IProfile
{
    string Title { get; set; }
    string Forenames { get; set; }
    string Surname { get; set; }
    DateTime DateOfBirth { get; set; }
    string Phone { get; set; }
    string Address { get; set; }
    double Height { get; set; }
    double Weight { get; set; }
    UserProfileGender Gender { get; set; }
    string ImageUrl { get; set; }
    string FullName { get; }
}