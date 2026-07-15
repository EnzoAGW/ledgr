namespace Ledgr.Application.Common.Exceptions;

public class NotFoundException(string message) : Exception(message);
public class ForbiddenException(string message = "Access denied.") : Exception(message);
public class UnauthorizedException(string message = "Invalid credentials.") : Exception(message);
public class ValidationException(string message) : Exception(message);
public class ConflictException(string message) : Exception(message);
