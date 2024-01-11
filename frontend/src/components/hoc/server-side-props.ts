export interface HocProps {
  api: string;
}

export function withServerSideProps() {
  return async (context: any) => {
    const props: HocProps = {
      api: process.env.API || "",
    };

    return { props };
  };
}
