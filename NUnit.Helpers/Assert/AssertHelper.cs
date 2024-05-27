using NUnit.Framework.Internal;

namespace NUnit.Helpers.Assert;

public static class AssertHelper
{
    public static void SpinAssert<T>(Func<T> func, Action<T> action)
    {
        Exception? exception = null;
        var success = SpinWait.SpinUntil(() =>
        {
            try
            {
                Framework.Assert.Multiple(() => action.Invoke(func.Invoke()));
            }
            catch (Exception? e)
            {
                exception = e;

                return false;
            }
            finally
            {
                TestExecutionContext.CurrentContext.CurrentResult.AssertionResults.Clear();
            }

            return true;
            //TODO: Add property to configure timeout
        }, TimeSpan.FromSeconds(5));

        if (!success)
        {
            throw exception!;
        }
    }
}