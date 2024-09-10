# Use the official Microsoft SQL Server 2016 image
FROM mcr.microsoft.com/azure-sql-edge:latest

# Set environment variables for SQL Server
ENV SA_PASSWORD=Test12345678*
ENV ACCEPT_EULA=Y

# Create a directory for the backup file
RUN mkdir -p /var/opt/mssql/backup

# Copy the backup file to the container
COPY back.bak /var/opt/mssql/backup/

# Copy the restore script to the container
COPY restore-database.sh /usr/src/app/

# Switch to the root user to change permissions
USER root

RUN curl https://packages.microsoft.com/keys/microsoft.asc | apt-key add - && \
    curl https://packages.microsoft.com/config/ubuntu/20.04/prod.list > /etc/apt/sources.list.d/mssql-release.list && \
    apt-get update && \
    ACCEPT_EULA=Y apt-get install -y msodbcsql17 mssql-tools unixodbc-dev && \
    echo 'export PATH="$PATH:/opt/mssql-tools/bin"' >> ~/.bashrc


# Make the restore script executable
RUN chmod +x /usr/src/app/restore-database.sh

# Set the stack size limit
#RUN echo "ulimit -s 8192" >> ~/.bashrc

# Switch back to the default user for SQL Server (usually mssql user)
USER mssql

# Run the restore script on container start
CMD /bin/bash /usr/src/app/restore-database.sh & /opt/mssql/bin/sqlservr
