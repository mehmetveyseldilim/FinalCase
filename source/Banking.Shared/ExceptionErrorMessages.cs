namespace Banking.Shared
{
    public static class ExceptionErrorMessages
    {
        public static string InvalidLoginErrorMessage => "User login validation has been failed. Incorrect username or password";

        public static string AccountNotFoundForGivenUserIdErrorMessage(int userId) => $"Account with user id {userId} does not exist in the database";

        public static string AccountNotFoundForGivenIdErrorMessage(int accountId) => $"Account with id {accountId} does not exist in the database";

        public static string InsufficientFundsErrorMessage => "Insufficient funds for the withdrawal.";

        public static string OperationLimitExceededErrorMessage(int accountId) => $"Operation limit exceeded for account with id {accountId}";

        public static string RefreshTokenBadRequestErrorMessage => "Invalid client request. The Token has some invalid values";

        public static string RoleNotFoundErrorMessage(string role) => $"Role with name: {role} doesn't exist in the database";

        public static string UserNotFoundForGivenUserId(int userId) => $"User with id: {userId} doesn't exist in the database";

        public static string UserNotFoundForGivenUserName(string userName) => $"User with name: {userName} does not exist in the database";

        public static string DailyLimitExceededErrorMessage(int accountId) => $"Daily spending limit exceeded for account with id {accountId}";
    }
}