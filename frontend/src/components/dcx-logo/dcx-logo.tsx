import Image from "next/image";

interface Props {}

const DcxLogo: React.FC<Props> = (props: Props) => {
  return <Image src="/img/brand-assets/dcx-logo.png" height={20} width={20} />;
};

export default DcxLogo;
