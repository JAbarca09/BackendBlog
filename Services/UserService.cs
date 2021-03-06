using System;
using System.Collections.Generic;
using System.Linq;//language Intergrated Query
using System.Threading.Tasks;
using Backend_Blog.Models;
using Backend_Blog.Models.DTO;
using Backend_Blog.Services.Context;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Backend_Blog.Services
{
    public class UserService : ControllerBase
    {
        private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            return _context.UserInfo;
        }

        public bool DoesUserExist(string? username)
        {
            //Check the table to see if the username exists.
            //Sinlge or default
            //If one item matches our condition that item will be returned
            //If no items match the condition a null will be returned
            //If multiple Items match the condition an error will occur
            // UserModel foundUser = _context.UserInfo.SingleOrDefault( user => user.Username == username);
            // if(foundUser != null)
            // {
            //     //the user does exists
            // }else{
            //     //The user does not exist in the table.
            // }

            return _context.UserInfo.SingleOrDefault(user => user.Username == username) != null;
        }

        public UserModel GetUserByUsername(string username)
        {
            return _context.UserInfo.SingleOrDefault(user => user.Username == username);
        }

         public UserIdDTO GetUserIdDTOByUsername(string username)
        {
            var UserInfo = new UserIdDTO();
            var foundUser = _context.UserInfo.SingleOrDefault(user => user.Username == username);
            UserInfo.UserId = foundUser.Id;
            UserInfo.PublisherName = foundUser.Username;
            return UserInfo;
        }

        public UserModel GetUserByID(int ID)
        {
            return _context.UserInfo.SingleOrDefault(user => user.Id == ID);
        }

        public IActionResult Login(LoginDTO user)
        {
            IActionResult Result = Unauthorized();
            //check to see if the user exist
            if (DoesUserExist(user.Username))
            {
                //true
                var foundUser = GetUserByUsername(user.Username);
                //check too see if the password is correct
                if (VerifyUserPassword(user.Password, foundUser.Hash, foundUser.Salt))
                {
                    var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("DayClassSuperDuperSecreteKey@209"));
                    var signinCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);
                    var tokeOptions = new JwtSecurityToken(
                        issuer: "http://localhost:5000",
                        audience: "http://localhost:5000",
                        claims: new List<Claim>(),
                        expires: DateTime.Now.AddMinutes(30),
                        signingCredentials: signinCredentials
                    );
                    var tokenString = new JwtSecurityTokenHandler().WriteToken(tokeOptions);
                    Result =  Ok(new { Token = tokenString });
                }
            }
            return Result;
        }

        public bool AddUser(CreateAccountDTO UserToAdd)
        {
            //If theuser already exist.
            //If they do not exist we can then have the account be created
            //Else Throw a false.
            bool result = false;
            if (!DoesUserExist(UserToAdd.Username))
            {
                //the user does exists
                UserModel newUser = new UserModel();
                var hashedPassword = HashPassword(UserToAdd.Password);
                newUser.Id = UserToAdd.Id;//0
                newUser.Username = UserToAdd.Username;//Angel
                newUser.Salt = hashedPassword.Salt;
                newUser.Hash = hashedPassword.Hash;

                _context.Add(newUser);
                result = _context.SaveChanges() != 0;
            }
            return result;
        }
        public PasswordDTO HashPassword(string? password)
        {
            PasswordDTO newHashedPassword = new PasswordDTO();
            byte[] SaltBytes = new byte[64];
            var provider = new RNGCryptoServiceProvider();
            provider.GetNonZeroBytes(SaltBytes);
            var Salt = Convert.ToBase64String(SaltBytes);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(password, SaltBytes, 10000);
            var Hash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
            newHashedPassword.Salt = Salt;
            newHashedPassword.Hash = Hash;
            return newHashedPassword;
        }

        public bool VerifyUserPassword(string? Password, string? StoredHash, string? StoredSalt)
        {
            var SaltBytes = Convert.FromBase64String(StoredSalt);
            var rfc2898DeriveBytes = new Rfc2898DeriveBytes(Password, SaltBytes, 10000);
            var newHash = Convert.ToBase64String(rfc2898DeriveBytes.GetBytes(256));
            return newHash == StoredHash;
        }

        public bool UpdateUser(UserModel userToUpdate)
        {
            //This one is sending over the whole object to be updated
            _context.Update<UserModel>(userToUpdate);
            return _context.SaveChanges() !=0; 
        }
        public bool UpdateUsername(int id, string Username)
        {
            //This one is sending over just the username.
            //Then you have to get the object to then be updated.
            UserModel foundUser = GetUserByID(id);
            bool result = false;
            if(foundUser != null)
            {
                //A user was foundUser
                foundUser.Username = Username;
                _context.Update<UserModel>(foundUser);
               result =  _context.SaveChanges() != 0;
            }
            return result;
        }

        public bool DeleteUser(string Username)
        {
            //This one is sending over just the username.
            //Then you have to get the object to then be updated.
            UserModel foundUser = GetUserByUsername(Username);
            bool result = false;
            if(foundUser != null)
            {
                //A user was foundUser
                foundUser.Username = Username;
                _context.Remove<UserModel>(foundUser);
               result =  _context.SaveChanges() != 0;
            }
            return result;
        }
    }
}