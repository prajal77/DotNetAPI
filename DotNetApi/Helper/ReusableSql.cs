using Dapper;
using DotNetApi.Data;
using DotNetApi.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace DotNetApi.Helper
{
    public class ReusableSql
    {
        private readonly DataContextDapper _dapper;
        public ReusableSql(IConfiguration _config)
        {
            _dapper = new DataContextDapper(_config);
        }
        public bool UpsertUser(UserComplete user)
        {
            string sql = @"
            EXEC TutorialAppSchema.spUser_Upsert 
                    @FirstName= @FirstNameParameter, 
                    @LastName= @LastNameParameter,
                    @Email= @EmailParameter, 
                    @Gender= @GenderParameter, 
                    @Active= @ActiveParameter,
                    @JobTitle= @JobTitleParameter,
                    @Department= @DepartmentParameter,
                    @Salary= @SalaryParameter, 
                    @UserId= @UserIdParameter";
            Console.WriteLine(sql);

            DynamicParameters sqlParameters = new DynamicParameters();
            sqlParameters.Add("@FirstNameParameter", user.FirstName, DbType.String);
            sqlParameters.Add("@LastNameParameter", user.LastName, DbType.String);
            sqlParameters.Add("@EmailParameter", user.Email, DbType.String);
            sqlParameters.Add("@GenderParameter", user.Gender, DbType.String);
            sqlParameters.Add("@ActiveParameter", user.Active, DbType.Boolean);
            sqlParameters.Add("@JobTitleParameter", user.JobTitle, DbType.String);
            sqlParameters.Add("@DepartmentParameter", user.Department, DbType.String);
            sqlParameters.Add("@SalaryParameter", user.Salary, DbType.Decimal);
            sqlParameters.Add("@UserIdParameter", user.UserId, DbType.Int32);


            return _dapper.ExecutSqlWithParameter(sql, sqlParameters);
        }   
    }
}
