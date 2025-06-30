const webpackMerge = require("webpack-merge");

const baseWebpackConfig = require("@kentico/xperience-webpack-config");

module.exports = (opts, argv) => {
  const baseConfig = (webpackConfigEnv, argv) => {
    return baseWebpackConfig({
      orgName: "xperience-community",
      projectName: "sql-browser",
      webpackConfigEnv: webpackConfigEnv,
      argv: argv,
    });
  };

  const projectConfig = {
    module: {
      rules: [
        {
          test: /\.(js|ts)x?$/,
          exclude: [/node_modules/],
          loader: "babel-loader",
        },
      ],
    },
    output: {
      clean: true
    },
    devServer: {
      port: 3009,
    },
  };

  return webpackMerge.merge(projectConfig, baseConfig(opts, argv));
};
