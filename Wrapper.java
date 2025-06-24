package com.k7it.wrapperPractice;

public class Wrapper {

	public static void main(String[] args) {
		
	
	//------>primitive - Wrapper->Boxing
		int i=10;
		@Deprecated
	//using Constructor 
		Integer i1=new Integer(i);
		System.out.println(i1);
	//using valurOf()
		Integer i2=Integer.valueOf(i);
		System.out.println(i2);
	//using Auto Boxing
		Integer i3=10;
		System.out.println(i3);
		
	//--------->Wrapper-Primitive->Un-boxing
		Integer j=new Integer(20);
		int j1=j.intValue();
		System.out.println(j1);
		//using Auto Un-Boxing
		int j2=new Integer(20);
		System.out.println(j2);
		
	//---------->String-primitive
		String s="30";
		int s1=Integer.parseInt(s);
		System.out.println(s1);
		short s2=Short.parseShort(s);
		System.out.println(s2);
		float s3=Float.parseFloat(s);
		System.out.println(s3);
		
	//---------->Primitive-String
		int k=40;
		Integer k2=Integer.valueOf(k);
		String s4=k2.toString();
		System.out.println(s4);
		
	//----------->String- Wrapper
		String s5="123";
		Integer l=Integer.valueOf(s5);
		System.out.println(l);
		
	//---------->Wrapper-String
		Integer m=Integer.valueOf(1234);
		String m1=m.toString();
		System.out.println(m1);
	}
}
