﻿// Copyright (c) 2025 Sergio Hernandez. All rights reserved.
//
//  Licensed under the Apache License, Version 2.0 (the "License").
//  You may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//      http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Common.Domain.Constants;

namespace TrackHub.Security.Infrastructure.SecurityDB.Configurations;

public class PolicyConfiguration : IEntityTypeConfiguration<Policy>
{
    public void Configure(EntityTypeBuilder<Policy> builder)
    {
        //Table name
        builder.ToTable(name: TableMetadata.Policy, schema: SchemaMetadata.Security);

        //Column names
        builder.Property(x => x.PolicyId).HasColumnName("id");
        builder.Property(x => x.Name).HasColumnName("name");
        builder.Property(x => x.Description).HasColumnName("description");

        builder.Property(t => t.Name)
            .HasMaxLength(ColumnMetadata.DefaultNameLength)
            .IsRequired();

        builder.Property(t => t.Description)
            .HasMaxLength(ColumnMetadata.DefaultDescriptionLength)
            .IsRequired();

    }
}
