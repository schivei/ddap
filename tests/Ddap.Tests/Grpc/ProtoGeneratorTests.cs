using Ddap.Core;
using Ddap.Grpc;
using Ddap.Grpc.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Ddap.Tests.Grpc;

public class ProtoGeneratorTests
{
    [Fact]
    public void GenerateProtoFile_Should_Include_Syntax_Declaration()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entity = CreateTestEntity("User", "dbo", 3);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("syntax = \"proto3\";");
    }

    [Fact]
    public void GenerateProtoFile_Should_Include_Namespace()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entity = CreateTestEntity("User", "dbo", 3);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("option csharp_namespace = \"Ddap.Grpc.Generated\";");
    }

    [Fact]
    public void GenerateProtoFile_Should_Include_Package_Name()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entity = CreateTestEntity("User", "dbo", 3);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("package dbo;");
    }

    [Fact]
    public void GenerateProtoFile_Should_Use_Default_Package_When_Schema_Is_Null()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entity = CreateTestEntity("User", null, 3);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("package ddap;");
    }

    [Fact]
    public void GenerateProtoFile_Should_Include_Entity_Message()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entity = CreateTestEntity("User", "dbo", 3);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("message User {");
    }

    [Fact]
    public void GenerateProtoFile_Should_Include_All_Properties()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("Id", typeof(int)),
            CreateProperty("Name", typeof(string)),
            CreateProperty("Age", typeof(int)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("int32 id = 1;");
        result.Should().Contain("string name = 2;");
        result.Should().Contain("int32 age = 3;");
    }

    [Fact]
    public void GenerateProtoFile_Should_Convert_Property_Names_To_CamelCase()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("FirstName", typeof(string)),
            CreateProperty("LastName", typeof(string)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("firstName");
        result.Should().Contain("lastName");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Int32_To_Proto_int32()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration> { CreateProperty("Age", typeof(int)) };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("int32 age");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Int64_To_Proto_int64()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("BigNumber", typeof(long)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("int64 bigNumber");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Boolean_To_Proto_bool()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("IsActive", typeof(bool)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("bool isActive");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Double_To_Proto_double()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("Price", typeof(double)),
        };
        var entity = CreateTestEntityWithProperties("Product", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("double price");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Float_To_Proto_float()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("Rating", typeof(Single)),
        };
        var entity = CreateTestEntityWithProperties("Product", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("float rating");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Decimal_To_Proto_double()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("Amount", typeof(decimal)),
        };
        var entity = CreateTestEntityWithProperties("Order", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("double amount");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_DateTime_To_Proto_string()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("CreatedDate", typeof(DateTime)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("string createdDate");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Guid_To_Proto_string()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("UserId", typeof(Guid)),
        };
        var entity = CreateTestEntityWithProperties("Session", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("string userId");
    }

    [Fact]
    public void GenerateProtoFile_Should_Include_Request_Messages()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entity = CreateTestEntity("User", "dbo", 1);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("message GetUserRequest");
        result.Should().Contain("message ListUserRequest");
        result.Should().Contain("message CreateUserRequest");
        result.Should().Contain("message UpdateUserRequest");
        result.Should().Contain("message DeleteUserRequest");
    }

    [Fact]
    public void GenerateProtoFile_Should_Include_Response_Messages()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entity = CreateTestEntity("User", "dbo", 1);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("message ListUserResponse");
        result.Should().Contain("repeated User items = 1;");
        result.Should().Contain("int32 total_count = 2;");
    }

    [Fact]
    public void GenerateProtoFile_Should_Include_Empty_Message()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entity = CreateTestEntity("User", "dbo", 1);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("message Empty {}");
    }

    [Fact]
    public void GenerateProtoFile_Should_Include_Service_Definition()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entity = CreateTestEntity("User", "dbo", 1);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("service UserService {");
        result.Should().Contain("rpc Get(GetUserRequest) returns (User);");
        result.Should().Contain("rpc List(ListUserRequest) returns (ListUserResponse);");
        result.Should().Contain("rpc Create(CreateUserRequest) returns (User);");
        result.Should().Contain("rpc Update(UpdateUserRequest) returns (User);");
        result.Should().Contain("rpc Delete(DeleteUserRequest) returns (Empty);");
    }

    [Fact]
    public void GenerateAllProtoFiles_Should_Include_Syntax_Declaration()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("User", "dbo", 2),
            CreateTestEntity("Product", "dbo", 3),
        };

        // Act
        var result = generator.GenerateAllProtoFiles(entities);

        // Assert
        result.Should().Contain("syntax = \"proto3\";");
    }

    [Fact]
    public void GenerateAllProtoFiles_Should_Include_All_Entities()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("User", "dbo", 2),
            CreateTestEntity("Product", "dbo", 3),
        };

        // Act
        var result = generator.GenerateAllProtoFiles(entities);

        // Assert
        result.Should().Contain("message User {");
        result.Should().Contain("message Product {");
    }

    [Fact]
    public void GenerateAllProtoFiles_Should_Use_Default_Package()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entities = new List<IEntityConfiguration> { CreateTestEntity("User", "dbo", 2) };

        // Act
        var result = generator.GenerateAllProtoFiles(entities);

        // Assert
        result.Should().Contain("package ddap;");
    }

    [Fact]
    public void GenerateAllProtoFiles_Should_Handle_Empty_Entity_List()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entities = new List<IEntityConfiguration>();

        // Act
        var result = generator.GenerateAllProtoFiles(entities);

        // Assert
        result.Should().Contain("syntax = \"proto3\";");
        result.Should().Contain("package ddap;");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Nullable_Int32_To_Proto_int32()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("NullableId", typeof(int?)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("int32 nullableId");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Nullable_Boolean_To_Proto_bool()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("IsActive", typeof(bool?)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("bool isActive");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Nullable_DateTime_To_Proto_string()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("CreatedDate", typeof(DateTime?)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("string createdDate");
    }

    [Fact]
    public void GenerateProtoFile_Should_Handle_Empty_Property_Name()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration> { CreateProperty("", typeof(string)) };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        // Note: This test documents current behavior where empty property names
        // result in invalid proto syntax. In production, property names should be validated.
        result.Should().Contain("string  = 1;"); // Current behavior with empty name
    }

    [Fact]
    public void GenerateProtoFile_Should_Handle_Single_Character_Property_Name()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration> { CreateProperty("X", typeof(string)) };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("string x = 1;");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Byte_To_Proto_int32()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration> { CreateProperty("Age", typeof(byte)) };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("int32 age");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Int16_To_Proto_int32()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("SmallNumber", typeof(short)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("int32 smallNumber");
    }

    [Fact]
    public void GenerateProtoFile_Should_Map_Unknown_Type_To_Proto_string()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("CustomType", typeof(object)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("string customType");
    }

    [Fact]
    public void GenerateProtoFile_Should_Handle_Entity_With_No_Properties()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var properties = new List<IPropertyConfiguration>();
        var entity = CreateTestEntityWithProperties("EmptyEntity", "dbo", properties);

        // Act
        var result = generator.GenerateProtoFile(entity);

        // Assert
        result.Should().Contain("message EmptyEntity {");
        result.Should().Contain("}");
        result.Should().Contain("service EmptyEntityService");
    }

    [Fact]
    public void GenerateAllProtoFiles_Should_Handle_Entities_With_No_Properties()
    {
        // Arrange
        var generator = new ProtoGenerator();
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntityWithProperties("Entity1", "dbo", new List<IPropertyConfiguration>()),
            CreateTestEntityWithProperties("Entity2", "dbo", new List<IPropertyConfiguration>()),
        };

        // Act
        var result = generator.GenerateAllProtoFiles(entities);

        // Assert
        result.Should().Contain("message Entity1 {");
        result.Should().Contain("message Entity2 {");
    }

    private static IPropertyConfiguration CreateProperty(string name, Type type)
    {
        var mock = new Mock<IPropertyConfiguration>();
        mock.Setup(p => p.PropertyName).Returns(name);
        mock.Setup(p => p.PropertyType).Returns(type);
        return mock.Object;
    }

    private static IEntityConfiguration CreateTestEntityWithProperties(
        string name,
        string? schema,
        List<IPropertyConfiguration> properties
    )
    {
        var mockEntity = new Mock<IEntityConfiguration>();
        mockEntity.Setup(e => e.EntityName).Returns(name);
        mockEntity.Setup(e => e.SchemaName).Returns(schema);
        mockEntity.Setup(e => e.Properties).Returns(properties);
        return mockEntity.Object;
    }

    private static IEntityConfiguration CreateTestEntity(
        string name,
        string? schema,
        int propertyCount
    )
    {
        var properties = new List<IPropertyConfiguration>();
        for (int i = 0; i < propertyCount; i++)
        {
            properties.Add(CreateProperty($"Property{i}", typeof(string)));
        }
        return CreateTestEntityWithProperties(name, schema, properties);
    }
}

public class ProtoFileControllerTests
{
    [Fact]
    public void Constructor_Should_Initialize_With_Repository()
    {
        // Arrange
        var mockRepository = new Mock<IEntityRepository>();

        // Act
        var controller = new ProtoFileController(mockRepository.Object);

        // Assert
        controller.Should().NotBeNull();
    }

    [Fact]
    public void GetAllProtos_Should_Return_File_Result()
    {
        // Arrange
        var mockRepository = new Mock<IEntityRepository>();
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("User", "dbo", 2),
            CreateTestEntity("Product", "dbo", 3),
        };
        mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        var controller = new ProtoFileController(mockRepository.Object);

        // Act
        var result = controller.GetAllProtos();

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("text/plain");
        fileResult.FileDownloadName.Should().Be("ddap.proto");
    }

    [Fact]
    public void GetAllProtos_Should_Include_All_Entities()
    {
        // Arrange
        var mockRepository = new Mock<IEntityRepository>();
        var entities = new List<IEntityConfiguration>
        {
            CreateTestEntity("User", "dbo", 2),
            CreateTestEntity("Product", "dbo", 3),
        };
        mockRepository.Setup(r => r.GetAllEntities()).Returns(entities);

        var controller = new ProtoFileController(mockRepository.Object);

        // Act
        var result = controller.GetAllProtos() as FileContentResult;
        var content = System.Text.Encoding.UTF8.GetString(result!.FileContents);

        // Assert
        content.Should().Contain("message User");
        content.Should().Contain("message Product");
    }

    [Fact]
    public void GetProto_Should_Return_File_Result_When_Entity_Exists()
    {
        // Arrange
        var mockRepository = new Mock<IEntityRepository>();
        var entity = CreateTestEntity("User", "dbo", 2);
        mockRepository.Setup(r => r.GetEntity("User")).Returns(entity);

        var controller = new ProtoFileController(mockRepository.Object);

        // Act
        var result = controller.GetProto("User");

        // Assert
        result.Should().BeOfType<FileContentResult>();
        var fileResult = result as FileContentResult;
        fileResult!.ContentType.Should().Be("text/plain");
        fileResult.FileDownloadName.Should().Be("User.proto");
    }

    [Fact]
    public void GetProto_Should_Return_NotFound_When_Entity_Does_Not_Exist()
    {
        // Arrange
        var mockRepository = new Mock<IEntityRepository>();
        mockRepository.Setup(r => r.GetEntity("NonExistent")).Returns((IEntityConfiguration?)null);

        var controller = new ProtoFileController(mockRepository.Object);

        // Act
        var result = controller.GetProto("NonExistent");

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
        var notFoundResult = result as NotFoundObjectResult;
        notFoundResult!.Value.Should().NotBeNull();
    }

    [Fact]
    public void GetProto_Should_Include_Entity_Definition()
    {
        // Arrange
        var mockRepository = new Mock<IEntityRepository>();
        var properties = new List<IPropertyConfiguration>
        {
            CreateProperty("Id", typeof(int)),
            CreateProperty("Name", typeof(string)),
        };
        var entity = CreateTestEntityWithProperties("User", "dbo", properties);
        mockRepository.Setup(r => r.GetEntity("User")).Returns(entity);

        var controller = new ProtoFileController(mockRepository.Object);

        // Act
        var result = controller.GetProto("User") as FileContentResult;
        var content = System.Text.Encoding.UTF8.GetString(result!.FileContents);

        // Assert
        content.Should().Contain("message User");
        content.Should().Contain("int32 id");
        content.Should().Contain("string name");
    }

    [Fact]
    public void GetProto_Should_Include_Service_Definition()
    {
        // Arrange
        var mockRepository = new Mock<IEntityRepository>();
        var entity = CreateTestEntity("User", "dbo", 1);
        mockRepository.Setup(r => r.GetEntity("User")).Returns(entity);

        var controller = new ProtoFileController(mockRepository.Object);

        // Act
        var result = controller.GetProto("User") as FileContentResult;
        var content = System.Text.Encoding.UTF8.GetString(result!.FileContents);

        // Assert
        content.Should().Contain("service UserService");
        content.Should().Contain("rpc Get(GetUserRequest)");
    }

    private static IPropertyConfiguration CreateProperty(string name, Type type)
    {
        var mock = new Mock<IPropertyConfiguration>();
        mock.Setup(p => p.PropertyName).Returns(name);
        mock.Setup(p => p.PropertyType).Returns(type);
        return mock.Object;
    }

    private static IEntityConfiguration CreateTestEntityWithProperties(
        string name,
        string? schema,
        List<IPropertyConfiguration> properties
    )
    {
        var mockEntity = new Mock<IEntityConfiguration>();
        mockEntity.Setup(e => e.EntityName).Returns(name);
        mockEntity.Setup(e => e.SchemaName).Returns(schema);
        mockEntity.Setup(e => e.Properties).Returns(properties);
        return mockEntity.Object;
    }

    private static IEntityConfiguration CreateTestEntity(
        string name,
        string? schema,
        int propertyCount
    )
    {
        var properties = new List<IPropertyConfiguration>();
        for (int i = 0; i < propertyCount; i++)
        {
            properties.Add(CreateProperty($"Property{i}", typeof(string)));
        }
        return CreateTestEntityWithProperties(name, schema, properties);
    }
}
