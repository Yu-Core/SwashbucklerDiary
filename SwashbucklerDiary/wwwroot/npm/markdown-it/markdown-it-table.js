import table from './table';

export const markdownItTable = (md, options) => {
  md.block.ruler.before('paragraph', 'table', table, {
    alt: ['paragraph', 'reference'],
  });
};
