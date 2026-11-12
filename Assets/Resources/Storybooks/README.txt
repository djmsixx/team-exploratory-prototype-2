Storybook scenes

The Introduction book is assigned to Assets/Scenes/IntroductionScene.
Assets/Scenes/ClimaxScene is assigned to the editable Climax.asset placeholder.
Add that book's pages and descriptions in order before using it. Keep the images
in a numbered folder, for example
Assets/Resources/Storybooks/Climax/Pages/1.png, 2.png, etc.

To open a book from another script:

StorybookSceneLoader.LoadStorybook(StorybookSceneLoader.IntroductionScene);
StorybookSceneLoader.LoadStorybook(StorybookSceneLoader.ClimaxScene);

Both scenes must be included in Build Settings. The storybook's Proceed option
loads GameScene by default; change nextSceneName on its Storybook component if
it should continue somewhere else.
