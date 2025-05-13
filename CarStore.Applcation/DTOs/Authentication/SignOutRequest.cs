using System.ComponentModel.DataAnnotations;

namespace CarStore.Application.DTOs;

public class SignOutRequest{
    [Required]
    public string Token { get; set; }
}