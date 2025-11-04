namespace DocumentationGenerator.Models.Declarations;

public struct InterfaceDeclaration
{
    public string Name { get; }
    public string? Definition { get; }
    public Declaration[]? Properties { get; set; }
    public Declaration[]? Methods { get; set; }

    public InterfaceDeclaration(string name, string? definition, Declaration[]? properties, Declaration[]? methods)
    {
        Name = name;
        Definition = definition;
        Properties = properties;
        Methods = methods;
    }
}