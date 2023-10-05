using System.Text.Json;

namespace CoreAPI.Utils;

public class OwnershipExistsException : Exception {

    public OwnershipExistsException(JsonDocument ownerships) : base(JsonSerializer.Serialize(new { message = "Found ownerships connected to property!", ownerships = ownerships })) {}

}

public class ChildPropertiesExistException :  Exception {

    public ChildPropertiesExistException(JsonElement childProperties) : base(JsonSerializer.Serialize(new { message = "Found child properties connected to property!", childProperties = childProperties })) { }
}
