class Reverse{
	int a=123;
	int sum=0;
	public static void main(String[]args){
		while(a>0){
			int rev=a%10;
			sum=sum*10+rev;
			a=a/10;
			}
		System.out.println(sum);
	}
}