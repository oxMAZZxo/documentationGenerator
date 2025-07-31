/// <summary>
/// Represents a Player in the game
/// </summary>
public class Player
{
    /// <summary>
    /// Readonly integer representing the players unique ID
    /// </summary>
    public int ID { get; }

    /// <summary>
    /// Readonly string representing 
    /// the players name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Default Player Constructor Initialising everything to default values
    /// </summary>
    public Player()
    {
        ID = -1;
        Name = "No Name";
    }

    /// <summary>
    /// Instantiates a Player with a given ID and name
    /// </summary>
    /// <param name="newID">The new players ID</param>
    /// <param name="newName">The new players name</param>
    public Player(int newID, string newName)
    {
        ID = newID;
        Name = newName;
    }

    /// <summary>
    /// Applies damage to this Player, deducting the given damage from their current health
    /// </summary>
    /// <param name="damage">The damage to apply</param>
    /// <returns>Returns true if the Player dies after this call is done, returns false otherwise.</returns>
    public bool TakeDamage(float damage)
    {
        // Comment
        return false;
    }


    /// <summary>
    /// Performs this players attack animation and logic.
    /// </summary>
    public void Attack()
    {

    }
}



public class Enemy
{
    /// <summary>
    /// The maximum health this Enemy can have
    /// </summary>
    public int MaxHealth { get; }
    /// <summary>
    /// Indicates whether this Enemy is dead.
    /// </summary>
    public bool IsDead { get; }

    /// <summary>
    /// Instantiates an Enemy with the given max health;
    /// </summary>
    /// <param name="maxHealth"></param>
    public Enemy(int maxHealth)
    {
        MaxHealth = maxHealth;
        IsDead = false;
    }


    /// <summary>
    /// Performs the attack animation and logic if a hit is registered.
    /// </summary>
    public void Attack()
    {
    }

    /// <summary>
    /// Applies the given damage to this Enemy's health
    /// </summary>
    /// <param name="damage">The damage to apply</param>
    public void TakeDamage(float damage)
    {

    }


}


public enum EnemyType
{
    Zombie,
    Soldier,
    Sniper
}