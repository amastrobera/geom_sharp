
namespace System {

  [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
  public class ExperimentalAttribute : Attribute {
    public string Message { get; }
    public ExperimentalAttribute(
        string message =
            "Warning: this function is a prototype, its implementation may be incomplete or not well tested")
        : base() {
      Message = message;
    }
  }

}
