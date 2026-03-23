// Import the shared library
@Library('college-ci-lib') _

pipeline {
    agent any

    // 1. Tell Jenkins to inject the .NET SDK into the pipeline's PATH
    tools {
        dotnetsdk 'dotnet-9' // Must match the EXACT name you gave it in Step 2
    }

    environment {
        DOCKER_IMAGE = 'ihany3c/college-control-system'
        DOCKER_CREDENTIALS_ID = 'dockerhub' // ID from Jenkins Credentials manager
        APP_VERSION = ''
    }

    stages {
        stage('Versioning') {
            steps {
                script {
                    // Call the custom step from the shared library
                    echo "The calculated version is ${calculateVersion()}"
                    APP_VERSION = calculateVersion()
                    currentBuild.displayName = "#${env.BUILD_NUMBER} - ${APP_VERSION}"
                }
            }
        }

        stage('Build') {
            steps {
                buildDotNet()
            }
        }

        stage('Test') {
            steps {
                testDotNet()
            }
        }

        // Only push to Docker Hub if we are on master or develop
        stage('Docker Build & Push') {
            when {
                anyOf {
                    branch 'master'
                    branch 'develop'
                }
            }
            steps {
                dockerBuildAndPush(env.DOCKER_IMAGE, env.APP_VERSION, env.DOCKER_CREDENTIALS_ID)
                
                // For master, also push a 'latest' tag
                script {
                    if (env.BRANCH_NAME == 'master') {
                        dockerBuildAndPush(env.DOCKER_IMAGE, 'latest', env.DOCKER_CREDENTIALS_ID)
                    }
                }
            }
        }

        // ---> UPDATED DEPLOY STAGE <---
        stage('Deploy with Compose') {
            when {
                anyOf { branch 'master'; branch 'develop' }
            }
            steps {
                script {
                    if (env.BRANCH_NAME == 'develop') {
                        // Pass 'Dev' and the version (e.g., 1.0.0-dev.5)
                        deployWithCompose('Development', env.APP_VERSION)
                    } else if (env.BRANCH_NAME == 'master') {
                        // Pass 'Prod' and the version (e.g., 1.0.0.5)
                        deployWithCompose('Production', env.APP_VERSION)
                    }
                }
            }
        }

        stage('Deploy') {
            when {
                anyOf {
                    branch 'master'
                    branch 'develop'
                }
            }
            steps {
                script {
                    if (env.BRANCH_NAME == 'develop') {
                        echo "Deploying to DEVELOPMENT environment..."
                        // Add your dev deployment script/command here
                    } else if (env.BRANCH_NAME == 'master') {
                        echo "Deploying to PRODUCTION environment..."
                        // Add your prod deployment script/command here
                    }
                }
            }
        }
    }
    
    post {
        always {
            cleanWs() // Clean workspace after build
        }
        success {
            echo "Pipeline succeeded!"
        }
        failure {
            echo "Pipeline failed. Check logs."
            // You could add an email or Slack notification here
        }
    }
}