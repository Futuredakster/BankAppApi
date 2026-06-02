using Xunit;

namespace BankApp
{
    // This prevents tests from running in parallel since they share static DataStore
    [CollectionDefinition("Sequential", DisableParallelization = true)]
    public class SequentialCollection
    {
    }
}
