using System;

namespace Serialize
{
   public abstract class DontSerializeAttribute : Attribute
   {
      protected DontSerializeAttribute() {}
   }
	
   public abstract class DontSerializeNullAttribute : Attribute
   {
      protected DontSerializeNullAttribute() {}
   }
}