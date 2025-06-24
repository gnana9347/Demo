package com.k7it.Assignment2;

public class Type_conversions {

	public static void main(String[] args) {

		//Implicit type conversions
				//byte to short
						byte d=20;
						short d1=d;
						System.out.println(d1);
				//short to integer
						int e1=d1;
						System.out.println(e1);
				//integer to long
						int a=10;
						long a1=a;
						System.out.println(a1);
				//long to float
						float b=a1;
						System.out.println(b);
				//float to double
						double c=b;
						System.out.println(c);
				
		//Explicit type conversion
				//Double to float
						double f1=2.25;
						float f2=(float)f1;
						System.out.println(f2);
				//float to long
						long f3=(long)f2;
						System.out.println(f3);
				//long to integer
						int f4=(int)f3;
						System.out.println(f4);
				//integer to short
						short f5=(short)f4;
						System.out.println(f5);
				//short to byte
						byte f6=(byte)f5;
						System.out.println(f6);
	}
}
