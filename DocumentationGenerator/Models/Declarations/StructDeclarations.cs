namespace DocumentationGenerator.Models.Declarations;

public struct StructDeclaration
{
    public string Name { get; }
    public string? Definition { get; }
    public Declaration[]? Properties { get; set; }
    public Declaration[]? Fields { get; set; }
    public Declaration[]? Methods { get; set; }

    public StructDeclaration(string name, string? defintion, Declaration[]? properties, Declaration[]? fields, Declaration[]? methods)
    {
        this.Name = name;
        this.Definition = defintion;
        this.Properties = properties;
        this.Fields = fields;
        this.Methods = methods;
    }

}