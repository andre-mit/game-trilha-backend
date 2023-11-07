﻿namespace GameTrilha.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; set; }
    public User User { get; set; }
    
    public Guid RoleId { get; set; }
    public Role Role { get; set; }

    public UserRole()
    {
        
    }

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}