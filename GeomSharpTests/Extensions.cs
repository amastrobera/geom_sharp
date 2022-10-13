using System;
using System.Threading;

namespace Microsoft.VisualStudio.TestTools.UnitTesting {

  /// <summary>
  /// Custom method attribute to repeat the test a Number of times
  /// Can be used everywhere Microsoft.VisualStudio.TestTools.UnitTesting is imported.
  /// </summary>
  public class RepeatedTestMethodAttribute : TestMethodAttribute {
    private int Number;
    public RepeatedTestMethodAttribute(int RepeatNumber) : base() {
      Number = RepeatNumber;
    }

    public override TestResult[] Execute(ITestMethod testMethod) {
      var result = new TestResult[Number];

      // one thread per method
      if (Thread.CurrentThread.GetApartmentState() == ApartmentState.STA) {
        for (int i = 0; i < Number; ++i) {
          Invoke(testMethod, out result[i]);
        }
        // Console.WriteLine("STA result (" + Number.ToString() + ")= \n" + result.ToString());
        return result;
      }

      // otherwise, create many threads
      for (int i = 0; i < Number; ++i) {
        ThreadPool.QueueUserWorkItem(new WaitCallback((_) => Invoke(testMethod, out result[i])), null);
      }

      // Console.WriteLine("MT result (" + Number.ToString() + ")= \n" + result.ToString());

      return result;
    }

    private void Invoke(ITestMethod testMethod, out TestResult result) {
      result = testMethod.Invoke(null);
    }
  }

  public class NonImplementedAttribute : TestMethodAttribute {
    public NonImplementedAttribute() : base() {}

    private void Invoke(ITestMethod testMethod, out TestResult result) {
      result = new TestResult();
      Console.WriteLine("Test " + DisplayName + " Not Implemented");
      result.Outcome = UnitTestOutcome.NotRunnable;
      result.ReturnValue = false;
    }
  }

}
