namespace DocumentationGenerator.MVVM.Model.Declarations
{
    public struct EnumDeclaration
    {
        public string Name { get; }
        public string? Definition { get; }
        public Declaration[] EnumMembers { get; }

        public EnumDeclaration(string name, string? definition, Declaration[] members)
        {
            Name = name;
            Definition = definition;
            EnumMembers = members;
        }
    }
}